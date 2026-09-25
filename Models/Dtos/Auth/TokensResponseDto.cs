namespace RiverLine.Api.Models.Dtos.Auth;

public record TokensResponseDto(
    string AccessToken,
    RefreshToken RefreshToken
    );