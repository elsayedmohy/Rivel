namespace RiverLine.Api.Models.Dtos.Auth;

public record AuthResponseDto(string Token, Guid UserId, string Role);