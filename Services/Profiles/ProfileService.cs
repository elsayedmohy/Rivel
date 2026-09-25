namespace RiverLine.Api.Services.Profiles;

public class ProfileService(
    UserManager<User> userManager,
    ApplicationDbContext dbContext,
    ILogger<ProfileService> logger) : IProfileService
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
                u.CarrierProfile != null ? u.CarrierProfile.Bio : null))
            .FirstOrDefaultAsync();

        if (profile is null)
            return Result.Failure(OperationError.NotFound, "user.not_found");

        return Result<ProfileDto>.Success(profile);
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
        foreach (var token in user.RefreshTokens ?? [])
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

    private static ProfileDto ToDto(User user) => new(
        user.Id,
        user.Name,
        user.Email!,
        user.PhoneNumber,
        user.Role,
        user.EmailConfirmed,
        user.CarrierProfile?.CompanyName,
        user.CarrierProfile?.Bio);
}
