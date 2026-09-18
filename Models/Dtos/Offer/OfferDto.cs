namespace RiverLine.Api.Models.Dtos.Offer;

public record OfferDto(
    Guid Id,
    Guid ShipmentRequestId,
    Guid? ShipmentId,
    Guid CarrierId,
    Guid VesselId,
    decimal Price,
    DateOnly ProposedPickupDate,
    string Status,
    string? CarrierName,
    string? CompanyName
);