namespace RiverLine.Api.Models.Dtos.ShipmentRequest;

public record CreateShipmentRequestDto(
    string CargoType,
    double Weight,
    Guid OriginNileBerthId,
    Guid DestinationNileBerthId,
    DateOnly RequestedDate
    );