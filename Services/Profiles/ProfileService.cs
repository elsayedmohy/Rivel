namespace RiverLine.Api.Services.Profiles;

public class ProfileService(
    UserManager<User> userManager,
    ApplicationDbContext dbContext,
    ILogger<ProfileService> logger,
    IFileStorage storage) : IProfileService
{
    public async Task<Result<ProfileDto>> GetMineAsync(Guid userId)
    {
        var profile = await dbContext.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new ProfileDto(
                u.Id,
                u.Name,
                u.Email!,
                u.PhoneNumber,
                u.Role,
                u.EmailConfirmed,
                u.CarrierProfile != null ? u.CarrierProfile.CompanyName : null,
                u.CarrierProfile != null ? u.CarrierProfile.Bio : null,
                u.CarrierProfile != null ? u.CarrierProfile.LogoPath : null))
            .FirstOrDefaultAsync();

        if (profile is null)
            return Result.Failure(OperationError.NotFound, "user.not_found");

        return Result<ProfileDto>.Success(profile with { LogoPath = storage.GetPublicUrl(profile.LogoPath) });
    }

    public async Task<Result<ProfileDto>> UpdateMineAsync(Guid userId, UpdateProfileDto dto)
    {
        var user = await dbContext.Users
            .Include(u => u.CarrierProfile)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
            return Result.Failure(OperationError.NotFound, "user.not_found");

        if (user.Role == UserRole.Carrier)
        {
            if (user.CarrierProfile is null)
                return Result.Failure(OperationError.NotFound, "carrier.profile_not_found");

            var company = dto.CompanyName?.Trim();
            if (string.IsNullOrEmpty(company))
                return Result.Failure(OperationError.Validation, "profile.company_required");

            user.CarrierProfile.CompanyName = company;
            user.CarrierProfile.Bio = string.IsNullOrWhiteSpace(dto.Bio) ? null : dto.Bio.Trim();
        }

        user.Name = dto.Name.Trim();

        var phone = PhoneNumbers.Normalize(dto.PhoneNumber);
        if (phone != user.PhoneNumber)
        {
            user.PhoneNumber = phone;
            user.PhoneNumberConfirmed = false;
        }

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            if (result.Errors.Any(e => e.Code == nameof(IdentityErrorDescriber.ConcurrencyFailure)))
                return Result.Failure(OperationError.Conflict, "profile.concurrent_update");

            logger.LogWarning("Profile update failed for {UserId}: {Errors}",
                userId, string.Join(", ", result.Errors.Select(e => e.Code)));
            return Result.Failure(OperationError.Validation, "profile.update_failed");
        }

        return Result<ProfileDto>.Success(ToDto(user));
    }

    public async Task<Result<bool>> ChangePasswordAsync(Guid userId, ChangePasswordDto dto)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Result.Failure(OperationError.NotFound, "user.not_found");

        var result = await userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
        if (!result.Succeeded)
        {
            if (result.Errors.Any(e => e.Code == nameof(IdentityErrorDescriber.PasswordMismatch)))
                return Result.Failure(OperationError.Validation, "password.current_incorrect");

            return Result.Failure(OperationError.Validation, "password.rejected");
        }

        var now = DateTime.UtcNow;
        var revoked = 0;
        foreach (var token in user.RefreshTokens)
        {
            if (!token.IsActive)
                continue;

            token.RevokedAt = now;
            revoked++;
        }

        if (revoked > 0)
            await userManager.UpdateAsync(user);

        logger.LogInformation("Password changed for {UserId}; revoked {Count} refresh token(s)",
            userId, revoked);

        return Result<bool>.Success(true);
    }


    public async Task<Result<ProfileDto>> UploadLogoAsync(Guid userId, IFormFile file, CancellationToken ct)
    {
        if (file.Length == 0)
            return Result.Failure(OperationError.Validation, "logo.empty");
        if (file.Length > ImageSignatures.MaxLogoBytes)
            return Result.Failure(OperationError.Validation, "logo.too_large");

        var user = await dbContext.Users
            .Include(u => u.CarrierProfile)
            .FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (user?.CarrierProfile is null)
            return Result.Failure(OperationError.NotFound, "carrier.profile_not_found");

        await using var stream = file.OpenReadStream();
        var header = new byte[ImageSignatures.HeaderLength];
        var read = await stream.ReadAtLeastAsync(header, header.Length, throwOnEndOfStream: false, ct);

        var format = ImageSignatures.Detect(header.AsSpan(0, read));
        if (format is null)
            return Result.Failure(OperationError.Validation, "logo.invalid_type");

        stream.Position = 0;

        var newPath = $"carriers/{userId}/{Guid.NewGuid():N}{format.Extension}";
        await storage.UploadAsync(newPath, stream, format.ContentType, ct);

        var oldPath = user.CarrierProfile.LogoPath;
        user.CarrierProfile.LogoPath = newPath;

        try
        {
            await dbContext.SaveChangesAsync(ct);
        }
        catch
        {
            await storage.TryDeleteAsync(newPath);
            throw;
        }

        if (oldPath is not null)
            await storage.TryDeleteAsync(oldPath);

        return Result<ProfileDto>.Success(ToDto(user));
    }

    public async Task<Result<ProfileDto>> DeleteLogoAsync(Guid userId)
    {
        var user = await dbContext.Users
            .Include(u => u.CarrierProfile)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user?.CarrierProfile is null)
            return Result.Failure(OperationError.NotFound, "carrier.profile_not_found");

        var oldPath = user.CarrierProfile.LogoPath;
        if (oldPath is not null)
        {
            user.CarrierProfile.LogoPath = null;
            await dbContext.SaveChangesAsync();
            await storage.TryDeleteAsync(oldPath);
        }

        return Result<ProfileDto>.Success(ToDto(user));
    }

    private ProfileDto ToDto(User user) => new(
        user.Id,
        user.Name,
        user.Email!,
        user.PhoneNumber,
        user.Role,
        user.EmailConfirmed,
        user.CarrierProfile?.CompanyName,
        user.CarrierProfile?.Bio,
        storage.GetPublicUrl(user.CarrierProfile?.LogoPath));
}