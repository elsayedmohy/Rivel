namespace RiverLine.Api.Mappers;

public class OfferMapper
{
    public OfferDto ToDto(Offer o) =>
        new(o.Id,
            o.ShipmentRequestId,
            o.Shipment is null ? null : o.Shipment.Id,
            o.CarrierId,
            o.VesselId,
            o.Price,
            o.ProposedPickupDate,
            o.Status.ToString(),
            o.Carrier?.Name,
            o.Carrier?.CarrierProfile?.CompanyName);
}