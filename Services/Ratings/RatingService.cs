namespace RiverLine.Api.Services.Ratings;



public class RatingService(ApplicationDbContext dbContext) : IRatingService
{

    public async Task<Result<RatingDto>> CreateAsync( Guid cargoOwnerId, CreateRatingDto dto)
    {
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

        return Result<RatingDto>.Success(new RatingDto(rating.Id, rating.ShipmentId, rating.Score, rating.Comment));
    }

    public async Task<Result<List<RatingDto>>> GetCarrierRatingsAsync(Guid carrierId)
    {
        var ratings = await dbContext.Ratings
            .AsNoTracking()
            .Where(r => r.CarrierId == carrierId)
            .Select(r => new RatingDto(
                r.Id,
                r.ShipmentId,
                r.Score,
                r.Comment))
            .ToListAsync();

        if (ratings.Count == 0)
            return Result<List<RatingDto>>.Failure(
                OperationError.NotFound,
                "Carrier has no ratings yet.");

        return Result<List<RatingDto>>.Success(ratings);
    }
}