using RiverLine.Api.Configurations;

namespace RiverLine.Api.Services.Ratings;

public class RatingService(
    ApplicationDbContext dbContext,
    INotificationService notifications,
    IOptions<AppUrls> urls) : IRatingService
{
    private readonly AppUrls _urls = urls.Value;

    public async Task<Result<RatingDto>> CreateAsync(Guid cargoOwnerId, CreateRatingDto dto)
    {
        var currnetUser = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == cargoOwnerId);
        if (currnetUser is null)
            return Result.Failure(OperationError.NotFound, "User not found.");

        var shipment = await dbContext.Shipments
            .Include(s => s.ShipmentRequest)
            .Include(s => s.Offer)
            .Include(s => s.Rating)
            .FirstOrDefaultAsync(s => s.Id == dto.ShipmentId);

        if (shipment is null)
            return Result.Failure(OperationError.NotFound, "Shipment not found.");

        if (shipment.ShipmentRequest.CargoOwnerId != cargoOwnerId)
            return Result.Failure(OperationError.Forbidden, "Not your shipment.");

        if (shipment.Status != ShipmentStatus.Delivered)
            return Result.Failure(OperationError.Conflict, "Shipment not delivered yet.");

        if (shipment.Rating is not null)
            return Result.Failure(OperationError.Conflict, "Shipment already rated.");

        var rating = new Rating
        {
            Id = Guid.NewGuid(),
            ShipmentId = dto.ShipmentId,
            CargoOwnerId = cargoOwnerId,
            CarrierId = shipment.Offer.CarrierId,
            Score = dto.Score,
            Comment = dto.Comment
        };

        shipment.IsRated = true;

        var profile = await dbContext.CarrierProfiles
            .FirstOrDefaultAsync(p => p.UserId == rating.CarrierId);
        if (profile is null)
            return Result.Failure(OperationError.NotFound, "Carrier not found.");
        profile.OverallRating =
            ((profile.OverallRating * profile.RatingCount) + dto.Score)
            / (profile.RatingCount + 1);
        profile.RatingCount++;

        dbContext.Ratings.Add(rating);
        await dbContext.SaveChangesAsync();
        await notifications.CreateAsync(new NotificationRequest(
            profile.UserId,
            NotificationType.RatingReceived,
            rating.Id,
            new
            {
                score = dto.Score,
                cargoOWnerName = currnetUser.Name,
                actionUrl = _urls.Rating()
            }));
        return Result<RatingDto>.Success(new RatingDto(rating.Id, rating.ShipmentId, rating.Score, rating.Comment));
    }

    public async Task<Result<CarrierRatingsDto>> GetMyRatingsAsync(
        Guid carrierId, CarrierRatingsQuery query)
    {
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 50);

        var mine = dbContext.Ratings
            .AsNoTracking()
            .Where(r => r.CarrierId == carrierId);

        var grouped = await mine
            .GroupBy(r => r.Score)
            .Select(g => new { Score = g.Key, Count = g.Count() })
            .ToListAsync();

        var distribution = Enumerable.Range(1, 5)
            .ToDictionary(s => s, s => grouped.FirstOrDefault(g => g.Score == s)?.Count ?? 0);

        var total = distribution.Values.Sum();
        var overall = total == 0
            ? 0m
            : Math.Round(distribution.Sum(d => (decimal)d.Key * d.Value) / total, 2);

        var items = await (
                from r in mine
                join u in dbContext.Users on r.CargoOwnerId equals u.Id
                orderby r.CreatedAt descending, r.Id
                select new ReceivedRatingDto(
                    r.Id,
                    r.Score,
                    r.Comment,
                    r.CreatedAt,
                    r.Shipment.ShipmentRequest.CargoType,
                    r.Shipment.ShipmentRequest.OriginNileBerth.ArabicName,
                    r.Shipment.ShipmentRequest.DestinationNileBerth.ArabicName,
                    u.Name))
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return Result<CarrierRatingsDto>.Success(
            new CarrierRatingsDto(overall,
                total,
                distribution,
                items,
                page,
                pageSize));
    }
}