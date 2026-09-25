namespace RiverLine.Api.Services.Auth;

public interface ITokenService
{
    string GenerateToken(User user);
    RefreshToken GenerateRefreshToken();
}