namespace RiverLine.Api.Models.Dtos.Auth;

public record RegisterDto(
    string Name,
    string Email,
    string Password,
    UserRole Role,
    string? CompanyName
    );