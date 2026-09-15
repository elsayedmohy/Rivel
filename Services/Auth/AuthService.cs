
namespace RiverLine.Api.Services.Auth;

public class AuthService(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    ITokenService tokenService,
    ApplicationDbContext dbContext)
    : IAuthService
{

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
            dbContext.CarrierProfiles.Add(new CarrierProfile
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                CompanyName = dto.CompanyName ?? string.Empty,
                OverallRating = 0
            });
            await dbContext.SaveChangesAsync();
        }

        var token = tokenService.GenerateToken(user);

        return new AuthResult(
            new AuthResponseDto(token, user.Id, user.Role.ToString()),
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

        var token = tokenService.GenerateToken(user);

        return new AuthResult(
            new AuthResponseDto(token, user.Id, user.Role.ToString()),
            []);
    }
}