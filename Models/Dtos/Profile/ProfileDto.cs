namespace RiverLine.Api.Models.Dtos.Profile;

public record ProfileDto(
    Guid Id,
    string Name,
    string Email,
    string? PhoneNumber,
    UserRole Role,
    bool EmailConfirmed,
    string? CompanyName,
    string? Bio);
