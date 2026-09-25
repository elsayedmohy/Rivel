namespace RiverLine.Api.Models.Dtos.Profile;

public record ChangePasswordDto(
    string CurrentPassword,
    string NewPassword);
