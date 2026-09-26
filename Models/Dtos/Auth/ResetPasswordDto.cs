namespace RiverLine.Api.Models.Dtos.Auth;

public record ResetPasswordDto(string Email, string Token, string NewPassword);