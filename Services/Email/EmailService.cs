namespace RiverLine.Api.Services.Email;


public class EmailService(
    IResend resend,                       
    IOptions<EmailSettings> settings,
    ILogger<EmailService> logger) : IEmailService
{
    private readonly EmailSettings _settings = settings.Value;
 
    public async Task SendAsync(EmailMessageDto message, CancellationToken ct = default)
    {
        var rendered = EmailRenderer.Render(message);
 
        if (rendered is null)
        {
            logger.LogError("Email template '{Key}' not found", message.TemplateKey);
            return;
        }
 
        var (subject, html) = rendered.Value;
 
        var email = new Resend.EmailMessage
        {
            From = $"{_settings.FromName} <{_settings.FromEmail}>",
            Subject = subject,
            HtmlBody = html
        };
 
        email.To.Add(message.To);
 
        await resend.EmailSendAsync(email, ct);
    }
}