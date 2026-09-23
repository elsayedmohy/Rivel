namespace RiverLine.Api.Models.Entities;

public class Vessel
{
    public Guid Id { get; set; }
    public Guid CarrierProfileId { get; set; }
    public CarrierProfile CarrierProfile { get; set; } = default!;
    public string Name { get; set; } = default!;
    public VesselType Type { get; set; }
    public string RegistrationNumber { get; set; } = default!;
    public decimal Capacity { get; set; }
    public VesselStatus Status { get; set; }
    public int? YearBuilt { get; set; }
 
    public bool IsArchived { get; set; }
    public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
    public ICollection<Offer> Offers { get; set; } = [];

}

