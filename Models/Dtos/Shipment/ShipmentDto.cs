namespace RiverLine.Api.Models.Dtos.Shipment;

public record ShipmentDto(
    Guid Id,
    string ShipmentStatus,
    Guid ShipmentRequestId,
    string CargoType,
    decimal Weight,
    NileBerthDto.NileBerthDto OriginNileBerth,
    NileBerthDto.NileBerthDto DestinationNileBerth,
    DateOnly RequestedDate,
    Guid CargoOwnerId,
    string CargoOwnerName,
    Guid OfferId,
    decimal OfferedPrice,
    DateOnly ProposedPickupDate,
    Guid VesselId,
    VesselType VesselType,
    string CarrierCompanyName,
    bool IsRated,
    ShipmentRatingDto? Rating
);

public record ShipmentRatingDto(int Score, string? Comment, DateTime CreatedAt);