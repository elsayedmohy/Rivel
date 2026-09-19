namespace RiverLine.Api.Models.Entities;

public class CarrierRoute
{
    public Guid Id { get; set; }
    public Guid CarrierProfileId { get; set; }
    public CarrierProfile CarrierProfile { get; set; } = default!;
    public string Origin { get; set; } = default!;
    public string Destination { get; set; } = default!;
    public bool IsActive { get; set; } = true;
}