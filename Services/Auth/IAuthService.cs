
namespace RiverLine.Api.Services.Auth;

public interface IAuthService
{ 
    Task<AuthResult> RegisterAsync(RegisterDto dto);
    Task<AuthResult?> LoginAsync(LoginDto dto);
    Task LogoutAsync(string token);
    
    Task<Result<TokensResponseDto>> RefreshTokenAsync(string token);

    Task<Result<bool>> ConfirmEmailAsync(ConfirmEmailDto dto);
     Task<Result<bool>> ResendConfirmationAsync(Guid userId);
    Task<Result<bool>> ResetPasswordAsync(ResetPasswordDto dto);
    Task ForgotPasswordAsync(string email);
}