namespace RiverLine.Api.Services.Notifications;

public class NotificationService(
    ApplicationDbContext dbContext,
    IHubContext<NotificationHub> hubContext,
    IEmailService emailService,
    ILogger<NotificationService> logger) : INotificationService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };
 
    public Task CreateAsync(NotificationRequest request, CancellationToken ct = default)
        => CreateManyAsync([request], ct);
 
    public async Task CreateManyAsync(
        IReadOnlyCollection<NotificationRequest> requests, CancellationToken ct = default)
    {
        if (requests.Count == 0)
            return;
 
        var now = DateTime.UtcNow;
 
        var entities = requests.Select(r => new Notification
        {
            Id = Guid.NewGuid(),
            UserId = r.UserId,
            Type = r.Type,
            EntityId = r.EntityId,
            DataJson = JsonSerializer.Serialize(r.Data, JsonOptions),
            IsRead = false,
            CreatedAt = now
        }).ToList();
 
        dbContext.Notifications.AddRange(entities);
        await dbContext.SaveChangesAsync(ct);
 
        await PushAsync(entities, ct);
 
        await EmailAsync(entities, ct);
    }
 
    private async Task PushAsync(IReadOnlyList<Notification> entities, CancellationToken ct)
    {
        foreach (var n in entities)
        {
            try
            {
                await hubContext.Clients
                    .User(n.UserId.ToString())
                    .SendAsync("notification", ToDto(n), ct);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex,
                    "SignalR push failed for notification {Id} to {UserId}", n.Id, n.UserId);
            }
        }
    }
 
    private async Task EmailAsync(IReadOnlyList<Notification> entities, CancellationToken ct)
    {
        var withEmail = entities
            .Select(n => (Notification: n, Spec: NotificationCatalog.EmailFor(n.Type)))
            .Where(x => x.Spec is not null)
            .ToList();
 
        if (withEmail.Count == 0)
            return;
 
        var userIds = withEmail.Select(x => x.Notification.UserId).Distinct().ToList();
 
        var recipients = await dbContext.Users
            .AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new { u.Id, u.Email, u.Name })
            .ToDictionaryAsync(u => u.Id, ct);
 
        foreach (var (notification, spec) in withEmail)
        {
            if (!recipients.TryGetValue(notification.UserId, out var user)
                || string.IsNullOrWhiteSpace(user.Email))
                continue;
 
            try
            {
                await emailService.SendAsync(new EmailMessageDto(
                    To: user.Email,
                    RecipientName: user.Name,
                    TemplateKey: spec!.TemplateKey,
                    Data: JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(
                              notification.DataJson, JsonOptions) ?? []),
                    ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Email failed for notification {Id} to {UserId}",
                    notification.Id, notification.UserId);
            }
        }
    }
 
    public async Task<Result<NotificationPageDto>> GetMineAsync(
        Guid userId, int page, int pageSize, bool unreadOnly, CancellationToken ct = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 50);
 
        var query = dbContext.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == userId);
 
        if (unreadOnly)
            query = query.Where(n => !n.IsRead);
 
        var items = await query
            .OrderByDescending(n => n.CreatedAt)
            .ThenBy(n => n.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize + 1)          // واحد زيادة عشان نعرف في كمان ولا لأ
            .ToListAsync(ct);
 
        var hasMore = items.Count > pageSize;
        if (hasMore)
            items.RemoveAt(items.Count - 1);
 
        var unread = await dbContext.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead, ct);
 
        return Result<NotificationPageDto>.Success(new NotificationPageDto(
            items.Select(ToDto).ToList(), unread, page, pageSize, hasMore));
    }
 
    public async Task<Result<int>> GetUnreadCountAsync(Guid userId, CancellationToken ct = default)
    {
        var count = await dbContext.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead, ct);
 
        return Result<int>.Success(count);
    }
 
    public async Task<Result<bool>> MarkReadAsync(
        Guid userId, Guid notificationId, CancellationToken ct = default)
    {
        var affected = await dbContext.Notifications
            .Where(n => n.Id == notificationId && n.UserId == userId && !n.IsRead)
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true), ct);
 
        return Result<bool>.Success(true);
    }
 
    public async Task<Result<bool>> MarkAllReadAsync(Guid userId, CancellationToken ct = default)
    {
        await dbContext.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true), ct);
 
        return Result<bool>.Success(true);
    }
 
    private static NotificationDto ToDto(Notification n)
        => new(n.Id, n.Type, n.EntityId,
               JsonDocument.Parse(n.DataJson).RootElement, n.IsRead, n.CreatedAt);
}