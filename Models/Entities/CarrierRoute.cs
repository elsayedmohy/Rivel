namespace RiverLine.Api.Models.Entities;

public class CarrierRoute
{
    public Guid Id { get; set; }
    public Guid CarrierProfileId { get; set; }
    public CarrierProfile CarrierProfile { get; set; } = default!;
    public Guid OriginNileBerthId { get; set; }
    public NileBerth OriginNileBerth { get; set; } = default!;
    public Guid DestinationNileBerthId { get; set; }
    public NileBerth DestinationNileBerth { get; set; } = default!;
    public bool IsActive { get; set; } = true;
}