namespace RiverLine.Api.Services.Email;


public class EmailService(IOptions<EmailSettings> settings) : IEmailService
{
    private readonly EmailSettings _settings = settings.Value;

    public async Task SendNewOfferNotificationAsync(
        string toEmail, string cargoOwnerName, string origin, string destination)
    {
        var client =  ResendClient.Create(_settings.ApiKey);

        var message = new EmailMessage
        {
            From = $"{_settings.FromName} <{_settings.FromEmail}>",
            To = [toEmail],
            Subject = "لديك عرض جديد على شحنتك",
            HtmlBody = $"""
                            <div dir="rtl" style="font-family: Arial, sans-serif;">
                                <h2>مرحباً {cargoOwnerName}</h2>
                                <p>لديك عرض جديد على شحنتك من <strong>{origin}</strong> إلى <strong>{destination}</strong>.</p>
                                <p>يرجى تسجيل الدخول لمراجعة العرض والرد عليه.</p>
                            </div>
                        """
        };

        await client.EmailSendAsync(message);
    }

    public async Task SendOfferAcceptedNotificationAsync(
        string toEmail, string carrierName, string origin, string destination, DateOnly pickupDate)
    {
        var client =  ResendClient.Create(_settings.ApiKey);

        var message = new EmailMessage
        {
            From = $"{_settings.FromName} <{_settings.FromEmail}>",
            To = [toEmail],
            Subject = "تم قبول عرضك",
            HtmlBody = $"""
                            <div dir="rtl" style="font-family: Arial, sans-serif;">
                                <h2>مبروك {carrierName}</h2>
                                <p>تم قبول عرضك للشحن من <strong>{origin}</strong> إلى <strong>{destination}</strong>.</p>
                                <p>تاريخ الاستلام: <strong>{pickupDate:dd/MM/yyyy}</strong></p>
                                <p>يرجى التواصل مع صاحب الشحنة لتنسيق التفاصيل.</p>
                            </div>
                        """
        };

        await client.EmailSendAsync(message);
    }
}