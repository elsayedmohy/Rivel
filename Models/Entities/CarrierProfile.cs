namespace RiverLine.Api.Models.Entities;

public class CarrierProfile
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;
    public string CompanyName { get; set; } = default!;
    public string? Bio { get; set; }
    public decimal OverallRating { get; set; } = 0;
    public int RatingCount { get; set; } = 0;
    public ICollection<CarrierRoute> Routes { get; set; } = new List<CarrierRoute>();

    public ICollection<Vessel> Vessels { get; set; } = new List<Vessel>();
}