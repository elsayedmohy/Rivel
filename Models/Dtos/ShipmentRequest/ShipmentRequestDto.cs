namespace RiverLine.Api.Models.Dtos.ShipmentRequest;

public record ShipmentRequestDto(
    Guid Id,
    string CargoType,
    decimal Weight,
    NileBerth OriginNileBerth,
    NileBerth DestinationNileBerth,
    DateOnly RequestedDate,
    string Status,
    Guid CargoOwnerId,
    int OffersCount
);