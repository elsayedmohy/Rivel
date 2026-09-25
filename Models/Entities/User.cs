

namespace RiverLine.Api.Models.Entities;

public class User : IdentityUser<Guid>
{
    public string Name { get; set; } = default!;
    public UserRole Role { get; set; } 

    public CarrierProfile? CarrierProfile { get; set; }
    public ICollection<ShipmentRequest> ShipmentRequests { get; set; } = new List<ShipmentRequest>();
    public ICollection<Offer> Offers { get; set; } = new List<Offer>();
   
    public ICollection<RefreshToken>? RefreshTokens { get; set; } 
}