namespace RiverLine.Api.Models.Dtos.Profile;

public record UpdateProfileDto(
    string Name,
    string? PhoneNumber,
    string? CompanyName,
    string? Bio);
