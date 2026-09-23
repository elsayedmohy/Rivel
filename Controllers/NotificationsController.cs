

namespace RiverLine.Api.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationsController(INotificationService notifications) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMine(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool unreadOnly = false,
        CancellationToken ct = default)
    {
        var result = await notifications.GetMineAsync(User.GetUserId(), page, pageSize, unreadOnly, ct);
        return this.ToActionResult(result);
    }
 
    [HttpGet("unread-count")]
    public async Task<IActionResult> UnreadCount(CancellationToken ct)
    {
        var result = await notifications.GetUnreadCountAsync(User.GetUserId(), ct);
        return this.ToActionResult(result);
    }
 
    [HttpPost("{id:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid id, CancellationToken ct)
    {
        var result = await notifications.MarkReadAsync(User.GetUserId(), id, ct);
        return this.ToActionResult(result);
    }

    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllRead(CancellationToken ct)
    {
        var result = await notifications.MarkAllReadAsync(User.GetUserId(), ct);
        return this.ToActionResult(result);
    }
}