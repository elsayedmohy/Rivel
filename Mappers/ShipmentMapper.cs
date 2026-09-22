namespace RiverLine.Api.Mappers;

public class ShipmentMapper(NileBerthMapper nileberthMapper)
{
    public ShipmentDto ToDto(Shipment s) => new(
        s.Id,
        s.Status.ToString(),
        s.ShipmentRequestId,
        s.ShipmentRequest.CargoType,
        s.ShipmentRequest.Weight,
        nileberthMapper.ToBerthDto(s.ShipmentRequest.OriginNileBerth),
        nileberthMapper.ToBerthDto(s.ShipmentRequest.DestinationNileBerth),
        s.ShipmentRequest.RequestedDate,
        s.ShipmentRequest.CargoOwnerId,
        s.ShipmentRequest.CargoOwner.Name,
        s.OfferId,
        s.Offer.Price,
        s.Offer.ProposedPickupDate,
        s.VesselId,
        s.Vessel.Type,
        s.Vessel.CarrierProfile.CompanyName,
        s.IsRated,
        s.Rating is null
            ? null
            : new ShipmentRatingDto(
                s.Rating.Score,
                s.Rating.Comment,
                s.Rating.CreatedAt
            )
    );
}