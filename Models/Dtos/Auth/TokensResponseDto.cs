namespace RiverLine.Api.Models.Dtos.Auth;

public record TokensResponseDto(
    string AccessToken,
    string RefreshToken
    );
    
public record RefreshTokenDto(string RefreshToken);