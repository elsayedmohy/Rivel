namespace RiverLine.Api.Models.Enums;

public enum NotificationType
{
    OfferReceived = 0,          
    OfferAccepted = 1,        
    OfferRejected = 2,        
    OfferWithdrawn = 3,       
    RequestMatched = 4,       
    RequestExpired = 5,      
    ShipmentStatusChanged = 6, 
    ShipmentCancelled = 7,     
    RatingReceived = 8,        
}