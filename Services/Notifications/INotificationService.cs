

namespace RiverLine.Api.Services.Notifications;

public interface INotificationService
{
    Task CreateAsync(NotificationRequest request, CancellationToken ct = default);
 
    Task CreateManyAsync(IReadOnlyCollection<NotificationRequest> requests,
        CancellationToken ct = default);
 
    Task<Result<NotificationPageDto>> GetMineAsync(
        Guid userId, int page, int pageSize, bool unreadOnly, CancellationToken ct = default);
 
    Task<Result<int>> GetUnreadCountAsync(Guid userId, CancellationToken ct = default);
    Task<Result<bool>> MarkReadAsync(Guid userId, Guid notificationId, CancellationToken ct = default);
    Task<Result<bool>> MarkAllReadAsync(Guid userId, CancellationToken ct = default);
}
