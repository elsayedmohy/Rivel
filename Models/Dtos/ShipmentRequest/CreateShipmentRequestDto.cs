namespace RiverLine.Api.Models.Dtos.ShipmentRequest;

public record CreateShipmentRequestDto(
    string CargoType,
    decimal Weight,
    Guid OriginNileBerthId,
    Guid DestinationNileBerthId,
    DateOnly RequestedDate
    );