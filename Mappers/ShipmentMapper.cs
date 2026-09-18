namespace RiverLine.Api.Mappers;

public class ShipmentMapper
{
    public ShipmentDto ToDto(Shipment s) => new(
        s.Id,
        s.Status.ToString(),
        s.ShipmentRequestId,
        s.ShipmentRequest.CargoType,
        s.ShipmentRequest.Weight,
        s.ShipmentRequest.Origin,
        s.ShipmentRequest.Destination,
        s.ShipmentRequest.RequestedDate,
        s.ShipmentRequest.CargoOwnerId,
        s.ShipmentRequest.CargoOwner.Name,
        s.OfferId,
        s.Offer.Price,
        s.Offer.ProposedPickupDate,
        s.VesselId,
        s.Vessel.Type,
        s.Vessel.CarrierProfile.CompanyName,
        s.Rating is null
            ? null
            : new RatingDto(
                s.Rating.Id,
                s.Rating.ShipmentId,
                s.Rating.Score,
                s.Rating.Comment)
    );
}