namespace RiverLine.Api.Services.Auth;

public class AuthService(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    IEmailService emailService,
    ITokenService tokenService,
    IOptions<AppUrls> urls,
    IMemoryCache cache,
    ApplicationDbContext dbContext,
    ILogger<AuthService> logger) : IAuthService
{
    //TODO : Currently I dont have same domain for the frontend and backend, so refreshToken cannot be stored as HTTPCookie for now  

    public async Task<AuthResult> RegisterAsync(RegisterDto dto)
    {
        var user = new User
        {
            UserName = dto.Email,
            Email = dto.Email,
            Name = dto.Name,
            Role = dto.Role
        };
        IdentityResult result = await userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return new AuthResult(null, result.Errors);

        if (dto.Role == UserRole.Carrier)
        {
            var profile = new CarrierProfile
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                CompanyName = dto.CompanyName!,
                OverallRating = 0
            };
            dbContext.CarrierProfiles.Add(profile);
            await dbContext.SaveChangesAsync();
        }

        var accessToken = tokenService.GenerateToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();
        await SendConfirmationEmailAsync(user);
        return new AuthResult(
            new AuthResponseDto(accessToken,
                refreshToken.Token,
                user.Id,
                user.Role.ToString()
            ),
            []);
    }

    public async Task<AuthResult?> LoginAsync(LoginDto dto)
    {
        var user = await userManager.FindByEmailAsync(dto.Email);
        if (user is null) return null;

        var result = await signInManager.CheckPasswordSignInAsync(user,
            dto.Password,
            lockoutOnFailure: true);
        if (!result.Succeeded)
            return null;

        var accessToken = tokenService.GenerateToken(user);
        var refreshToken = await HandleRefreshToken(user);

        return new AuthResult(
            new AuthResponseDto(accessToken, refreshToken.Token, user.Id, user.Role.ToString()),
            []);
    }

    public async
        Task<Result<TokensResponseDto>> RefreshTokenAsync(string token)
    {
        var user = await userManager.Users.SingleOrDefaultAsync(user =>
            user.RefreshTokens.Any(t => t.Token == token));
        if (user is null)
            return Result.Failure(OperationError.Unauthorized, "Unauthorized");

        var refreshToken = user.RefreshTokens.Single(t => t.Token == token);
        if (!refreshToken.IsActive)
            return Result.Failure(OperationError.Unauthorized, "Unauthorized");

        refreshToken.RevokedAt = DateTime.UtcNow;
        var newRefreshToken = tokenService.GenerateRefreshToken();
        user.RefreshTokens.Add(newRefreshToken);
        await userManager.UpdateAsync(user);

        var accessToken = tokenService.GenerateToken(user);

        return Result<TokensResponseDto>.Success(
            new TokensResponseDto(
                AccessToken: accessToken,
                RefreshToken: newRefreshToken.Token
            )
        );
    }


    private async Task<RefreshToken> HandleRefreshToken(User user)
    {
        RefreshToken refreshToken;
        if (user.RefreshTokens.Any(t => t.IsActive))
        {
            RefreshToken activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
            refreshToken = activeRefreshToken;
        }
        else
        {
            refreshToken = tokenService.GenerateRefreshToken();
            user.RefreshTokens.Add(refreshToken);
            await userManager.UpdateAsync(user);
        }

        return refreshToken!;
    }

    public async Task<Result<bool>> ConfirmEmailAsync(ConfirmEmailDto dto)
    {
        var user = await userManager.FindByIdAsync(dto.UserId.ToString());
        if (user is null)
            return Result.Failure(OperationError.Validation, "auth.confirm_invalid");

        if (user.EmailConfirmed)
            return Result<bool>.Success(true);

        var token = IdentityTokens.Decode(dto.Token);
        if (token is null)
            return Result.Failure(OperationError.Validation, "auth.confirm_invalid");

        var result = await userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
            return Result.Failure(OperationError.Validation, "auth.confirm_invalid");

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> ResendConfirmationAsync(Guid userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Result.Failure(OperationError.NotFound, "user.not_found");

        if (!user.EmailConfirmed && TryStartCooldown($"confirm:{user.Id}"))
            await SendConfirmationEmailAsync(user);

        return Result<bool>.Success(true);
    }

    public async Task ForgotPasswordAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user is null || !TryStartCooldown($"reset:{user.Id}"))
            return;

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var link = urls.Value.ResetPassword(user.Email!, IdentityTokens.Encode(token));
        await SendSafelyAsync(user, "reset-password", new { actionUrl = link });
    }

    public async Task<Result<bool>> ResetPasswordAsync(ResetPasswordDto dto)
    {
        var user = await userManager.FindByEmailAsync(dto.Email);
        var token = IdentityTokens.Decode(dto.Token);

        if (user is null || token is null)
            return Result.Failure(OperationError.Validation, "auth.reset_invalid");

        var result = await userManager.ResetPasswordAsync(user, token, dto.NewPassword);
        if (!result.Succeeded)
        {
            if (result.Errors.Any(e => e.Code == nameof(IdentityErrorDescriber.InvalidToken)))
                return Result.Failure(OperationError.Validation, "auth.reset_invalid");

            return Result.Failure(OperationError.Validation, "password.rejected");
        }

        user.EmailConfirmed = true;

        user.LockoutEnd = null;
        user.AccessFailedCount = 0;

        foreach (var t in user.RefreshTokens ?? [])
            if (t.IsActive)
                t.RevokedAt = DateTime.UtcNow;

        await userManager.UpdateAsync(user);
        return Result<bool>.Success(true);
    }

    private async Task SendConfirmationEmailAsync(User user)
    {
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var link = urls.Value.ConfirmEmail(user.Id, IdentityTokens.Encode(token));
        await SendSafelyAsync(user, "confirm-email", new { actionUrl = link });
    }

    private async Task SendSafelyAsync(User user, string template, object data)
    {
        try
        {
            await emailService.SendAsync(
                new EmailMessageDto(user.Email!, user.Name, template, EmailData.From(data)));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send {Template} email to {UserId}", template, user.Id);
        }
    }

    private bool TryStartCooldown(string key)
    {
        if (cache.TryGetValue(key, out _))
            return false;

        cache.Set(key, true, TimeSpan.FromMinutes(5));
        return true;
    }
}