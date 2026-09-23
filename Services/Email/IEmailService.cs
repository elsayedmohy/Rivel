namespace RiverLine.Api.Services.Email;

public interface IEmailService
{
    Task SendAsync(EmailMessageDto message, CancellationToken ct = default);
}