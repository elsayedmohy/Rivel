namespace RiverLine.Api.Models.Entities;

[Owned] 
public class RefreshToken
{
    public string Token { get; set; }
    public DateTime ExpireTime { get; set; }
    public bool  IsExpired  => DateTime.UtcNow >= ExpireTime;
    public DateTime CreatedAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public bool IsActive => RevokedAt == null &&  !IsExpired;
}