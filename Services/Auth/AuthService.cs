namespace RiverLine.Api.Services.Auth;

public class AuthService(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    ITokenService tokenService,
    ApplicationDbContext dbContext)
    : IAuthService
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
        var refreshToken = await HandleRefreshToken(user);

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
                RefreshToken: newRefreshToken
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
}