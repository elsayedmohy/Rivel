namespace RiverLine.Api.Models.Entities;

public class Offer
{
    public Guid Id { get; set; }
    public Guid ShipmentRequestId { get; set; }
    public ShipmentRequest ShipmentRequest { get; set; } = default!;
    public Guid CarrierId { get; set; }
    public User Carrier { get; set; } = default!;
    public decimal Price { get; set; }
    public DateTime ProposedPickupDate { get; set; }
    public OfferStatus Status { get; set; } = OfferStatus.Pending;
    public Shipment? Shipment { get; set; }

}

