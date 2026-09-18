namespace RiverLine.Api.Models.Dtos.Shipment;

public record ShipmentDto(
    Guid Id,
    string ShipmentStatus,
    Guid ShipmentRequestId,
    string CargoType,
    double Weight,
    string Origin,
    string Destination,
    DateOnly RequestedDate,
    Guid CargoOwnerId,
    string CargoOwnerName,
    Guid OfferId,
    decimal OfferedPrice,
    DateOnly ProposedPickupDate,
    Guid VesselId,
    string VesselType,
    string CarrierCompanyName,
    RatingDto? Rating
);