namespace RiverLine.Api.Models.Entities;

public class NileBerth
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string ArabicName { get; set; } = null!;
    public string Governorate { get; set; } = null!;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public BerthType Type { get; set; }
    public NavigationAxis Axis { get; set; }
    public CoordinateAccuracy CoordinateAccuracy { get; set; }
    public bool IsActive { get; set; } = true;
}