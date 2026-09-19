namespace RiverLine.Api.Models.Dtos.Shipment;

public record ShipmentDto(
    Guid Id,
    string ShipmentStatus,
    Guid ShipmentRequestId,
    string CargoType,
    double Weight,
    NileBerth OriginNileBerth,
    NileBerth DestinationNileBerth,
    DateOnly RequestedDate,
    Guid CargoOwnerId,
    string CargoOwnerName,
    Guid OfferId,
    decimal OfferedPrice,
    DateOnly ProposedPickupDate,
    Guid VesselId,
    VesselType VesselType,
    string CarrierCompanyName,
    RatingDto? Rating
);