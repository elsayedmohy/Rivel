namespace RiverLine.Api.Services.Email;

public interface IEmailService
{
    Task SendNewOfferNotificationAsync(string toEmail, string cargoOwnerName, string origin, string destination);
    Task SendOfferAcceptedNotificationAsync(string toEmail, string carrierName, string origin, string destination, DateOnly pickupDate);
}