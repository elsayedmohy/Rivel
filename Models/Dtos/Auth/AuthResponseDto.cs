namespace RiverLine.Api.Models.Dtos.Auth;

public record AuthResponseDto(
    string Token,
    string RefreshToken,
    Guid UserId,
    string Role
    );