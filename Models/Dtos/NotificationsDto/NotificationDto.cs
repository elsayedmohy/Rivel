namespace RiverLine.Api.Models.Dtos.NotificationsDto;

public sealed record NotificationRequest(
    Guid UserId,
    NotificationType Type,
    Guid EntityId,
    object Data);       
 
public sealed record NotificationDto(
    Guid Id,
    string Type,
    Guid EntityId,
    JsonElement Data,
    bool IsRead,
    DateTime CreatedAt);
 
public sealed record NotificationPageDto(
    IReadOnlyList<NotificationDto> Items,
    int UnreadCount,
    int Page,
    int PageSize,
    bool HasMore);