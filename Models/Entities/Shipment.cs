namespace RiverLine.Api.Models.Entities;

public class Shipment
{
    public Guid Id { get; set; }
    public Guid ShipmentRequestId { get; set; }
    public ShipmentRequest ShipmentRequest { get; set; } = default!;
    public Guid OfferId { get; set; }
    public Offer Offer { get; set; } = default!;
    public Guid VesselId { get; set; }
    public Vessel Vessel { get; set; } = default!;
    public ShipmentStatus Status { get; set; } = ShipmentStatus.Matched;
    public bool IsRated { get; set; }
    public Rating? Rating { get; set; }
}

