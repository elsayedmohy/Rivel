namespace RiverLine.Api.Models.Dtos.Auth;

public sealed record AuthResult(
    AuthResponseDto? Response,
    IEnumerable<IdentityError> Errors)
{
    public bool Succeeded => Response is not null;
}