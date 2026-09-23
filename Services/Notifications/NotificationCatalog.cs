namespace RiverLine.Api.Services.Notifications;

public sealed record EmailSpec(string TemplateKey);
 
public static class NotificationCatalog
{
    private static readonly Dictionary<NotificationType, EmailSpec> Emails = new()
    {
        [NotificationType.OfferReceived] =
            new("offer-received"),
        [NotificationType.OfferAccepted] =
            new("offer-accepted"),
        [NotificationType.ShipmentCancelled] =
            new("shipment-cancelled"),
 
    };
 
    public static EmailSpec? EmailFor(NotificationType type)
        => Emails.GetValueOrDefault(type);
}