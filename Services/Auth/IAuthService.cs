
namespace RiverLine.Api.Services.Auth;

public interface IAuthService
{ 
    Task<AuthResult> RegisterAsync(RegisterDto dto);
    Task<AuthResult?> LoginAsync(LoginDto dto);
    Task<Result<TokensResponseDto>> RefreshTokenAsync(string token);
}