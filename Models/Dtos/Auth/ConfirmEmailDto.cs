namespace RiverLine.Api.Models.Dtos.Auth;

public record ConfirmEmailDto(Guid UserId, string Token);
