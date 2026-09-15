namespace RiverLine.Api.Mappers;

public class ShipmentRequestMapper
{
    public ShipmentRequestDto ToDto(ShipmentRequest r) =>
        new(r.Id,
            r.CargoType,
            r.Weight,
            r.Origin,
            r.Destination,
            r.RequestedDate,
            r.Status.ToString(),
            r.CargoOwnerId,
            r.Offers?.Count ?? 0);
}