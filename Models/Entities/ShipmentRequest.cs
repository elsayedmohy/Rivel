namespace RiverLine.Api.Models.Entities;

public class ShipmentRequest
{
    public Guid Id { get; set; }
    public Guid CargoOwnerId { get; set; }
    public User CargoOwner { get; set; } = default!;
    public string CargoType { get; set; } = default!;
    public double Weight { get; set; }
    public Guid OriginNileBerthId { get; set; }
    public NileBerth OriginNileBerth { get; set; } = default!;
    public Guid DestinationNileBerthId { get; set; }
    public NileBerth DestinationNileBerth { get; set; } = default!;
    public DateOnly RequestedDate { get; set; }
    public ShipmentRequestStatus Status { get; set; } = ShipmentRequestStatus.Open;

    public ICollection<Offer> Offers { get; set; } = new List<Offer>();
    public Shipment? Shipment { get; set; }
    public DateTime CreatedAt { get; set; } =  DateTime.UtcNow;
}

