namespace RiverLine.Api.Models.Entities;

public class Rating
{
    public Guid Id { get; set; }
    public Guid ShipmentId { get; set; }
    public Shipment Shipment { get; set; } = default!;
    public Guid CargoOwnerId { get; set; }
    public Guid CarrierId { get; set; }
    public int Score { get; set; }
    public string? Comment { get; set; }
}