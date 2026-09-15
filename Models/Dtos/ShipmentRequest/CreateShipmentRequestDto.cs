namespace RiverLine.Api.Models.Dtos.ShipmentRequest;

public record CreateShipmentRequestDto(
    string CargoType,
    double Weight,
    string Origin,
    string Destination,
    DateTime RequestedDate);