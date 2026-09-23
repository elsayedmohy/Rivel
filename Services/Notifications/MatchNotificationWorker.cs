using RiverLine.Api.Configurations;

namespace RiverLine.Api.Services.Notifications;

public class MatchNotificationWorker(
    MatchNotificationQueue queue,
    IServiceScopeFactory scopeFactory,
    ILogger<MatchNotificationWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await foreach (var requestId in queue.ReadAllAsync(ct))
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var notifications = scope.ServiceProvider
                    .GetRequiredService<INotificationService>();
                var urls = scope.ServiceProvider.GetRequiredService<IOptions<AppUrls>>().Value;
 
                await NotifyMatchesAsync(db, notifications, urls, requestId, ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Match notification failed for {RequestId}", requestId);
            }
        }
    }
 
    private static async Task NotifyMatchesAsync(
        ApplicationDbContext db,
        INotificationService notifications,
        AppUrls urls,
        Guid requestId,
        CancellationToken ct)
    {
        var request = await db.ShipmentRequests
            .AsNoTracking()
            .Include(r => r.OriginNileBerth)
            .Include(r => r.DestinationNileBerth)
            .FirstOrDefaultAsync(r => r.Id == requestId, ct);
 
        if (request is null || request.Status != ShipmentRequestStatus.Open)
            return;
 
        var carrierIds = await db.CarrierRoutes
            .AsNoTracking()
            .Where(r => r.IsActive
                     && r.OriginNileBerthId == request.OriginNileBerthId
                     && r.DestinationNileBerthId == request.DestinationNileBerthId)
            .Where(r => r.CarrierProfile.Vessels.Any(v =>
                     !v.IsArchived
                     && v.Status == VesselStatus.Available
                     && v.Capacity >= request.Weight))
            .Select(r => r.CarrierProfile.UserId)
            .Distinct() 
            .ToListAsync(ct);
 
        if (carrierIds.Count == 0)
            return;
 
        var data = new
        {
            cargoType   = request.CargoType,
            weight      = request.Weight,
            origin      = request.OriginNileBerth.ArabicName,
            destination = request.DestinationNileBerth.ArabicName,
            pickupDate  = request.RequestedDate,
            actionUrl   = urls.Suggested()
        };
 
        await notifications.CreateManyAsync(
            carrierIds.Select(id => new NotificationRequest(
                id, NotificationType.RequestMatched, request.Id, data)).ToList(),
            ct);
    }
}