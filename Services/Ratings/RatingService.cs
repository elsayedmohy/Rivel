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
        var currentUser = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == cargoOwnerId);
        if (currentUser is null)
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

        var carrierExists = await dbContext.CarrierProfiles.AnyAsync(p => p.UserId == shipment.Offer.CarrierId);
        if (!carrierExists)
            return Result.Failure(OperationError.NotFound, "carrier.not_found");


        dbContext.Ratings.Add(rating);
        await dbContext.SaveChangesAsync();
        await RecalculateCarrierRatingAsync(rating.CarrierId);
        await notifications.CreateAsync(new NotificationRequest(
            shipment.Offer.CarrierId,
            NotificationType.RatingReceived,
            rating.Id,
            new
            {
                score = dto.Score,
                cargoOWnerName = currentUser.Name,
                actionUrl = _urls.Rating()
            }));
        return Result<RatingDto>.Success(new RatingDto(rating.Id, rating.ShipmentId, rating.Score, rating.Comment));
    }

    public async Task<Result<CarrierRatingsDto>> GetMyRatingsAsync(
        Guid carrierId, CarrierRatingsQuery query)
    {
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 50);

        var summary = await dbContext.CarrierProfiles
            .AsNoTracking()
            .Where(p => p.UserId == carrierId)
            .Select(p => new { p.OverallRating, p.RatingCount })
            .FirstOrDefaultAsync();

        if (summary is null)
            return Result.Failure(OperationError.NotFound, "carrier.not_found");
        
        var mine = dbContext.Ratings
            .AsNoTracking()
            .Where(r => r.CarrierId == carrierId);

        var grouped = await mine
            .GroupBy(r => r.Score)
            .Select(g => new { Score = g.Key, Count = g.Count() })
            .ToListAsync();

        var distribution = Enumerable.Range(1, 5)
            .ToDictionary(s => s, s => grouped.FirstOrDefault(g => g.Score == s)?.Count ?? 0);


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

        items = items
            .Select(i => i with { CargoOwnerName = FirstName(i.CargoOwnerName) })
            .ToList();
        
        return Result<CarrierRatingsDto>.Success(
            new CarrierRatingsDto(summary.OverallRating,
                summary.RatingCount,
                distribution,
                items,
                page,
                pageSize));
    }

    private static string FirstName(string fullName) =>
        fullName.Trim()
            .Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault() ?? "";

    private Task RecalculateCarrierRatingAsync(Guid carrierUserId) =>
        dbContext.CarrierProfiles
            .Where(p => p.UserId == carrierUserId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.RatingCount,
                    _ => dbContext.Ratings.Count(r => r.CarrierId == carrierUserId))
                .SetProperty(p => p.OverallRating,
                    _ => Math.Round(
                        dbContext.Ratings
                            .Where(r => r.CarrierId == carrierUserId)
                            .Average(r => (decimal?)r.Score) ?? 0m, 2)));
}