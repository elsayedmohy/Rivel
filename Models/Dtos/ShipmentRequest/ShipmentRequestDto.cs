namespace RiverLine.Api.Models.Dtos.ShipmentRequest;

public record ShipmentRequestDto(
    Guid Id,
    string CargoType,
    double Weight,
    string Origin,
    string Destination,
    DateOnly RequestedDate,
    string Status,
    Guid CargoOwnerId,
    int OffersCount
);