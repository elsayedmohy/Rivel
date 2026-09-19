namespace RiverLine.Api.Mappers;

public class ShipmentRequestMapper
{
    public ShipmentRequestDto ToDto(ShipmentRequest r) =>
        new(r.Id,
            r.CargoType,
            r.Weight,
            r.OriginNileBerth,
            r.DestinationNileBerth,
            r.RequestedDate,
            r.Status.ToString(),
            r.CargoOwnerId,
            r.Offers.Count);
}