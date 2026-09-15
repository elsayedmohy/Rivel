namespace RiverLine.Api.Models.Dtos.Shipment;

public enum ShipmentOperationError
{
    NotFound,
    Forbidden,
    Conflict,
    Invalid
}

public record ShipmentOperationResult(
    bool Succeeded,
    ShipmentDto? Data,
    ShipmentOperationError? Error,
    string? Message)
{
    public static ShipmentOperationResult Success(ShipmentDto data) =>
        new(true, data, null, null);

    public static ShipmentOperationResult Failure(
        ShipmentOperationError error,
        string message) =>
        new(false, null, error, message);
}