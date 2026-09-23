namespace RiverLine.Api.Models.Entities;

public class Notification
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public NotificationType Type { get; set; }
    public Guid EntityId { get; set; }
    public string DataJson { get; set; } = "{}";
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}