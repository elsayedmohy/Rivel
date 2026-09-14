namespace RiverLine.Api.Models.Entities;

public class Vessel
{
    public Guid Id { get; set; }
    public Guid CarrierProfileId { get; set; }
    public CarrierProfile CarrierProfile { get; set; } = default!;
    public double Capacity { get; set; }
    public string Type { get; set; } = default!;
    public VesselStatus Status { get; set; }

    public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
}

