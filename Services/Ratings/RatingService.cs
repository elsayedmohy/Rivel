namespace RiverLine.Api.Services.Ratings;



public class RatingService(ApplicationDbContext dbContext) : IRatingService
{

    public async Task<Result<RatingDto>> CreateAsync(Guid shipmentId, Guid cargoOwnerId, CreateRatingDto dto)
    {
        var shipment = await dbContext.Shipments
            .Include(s => s.ShipmentRequest)
            .Include(s => s.Offer)
            .Include(s => s.Rating)
            .FirstOrDefaultAsync(s => s.Id == shipmentId);

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
            ShipmentId = shipmentId,
            CargoOwnerId = cargoOwnerId,
            CarrierId = shipment.Offer.CarrierId,
            Score = dto.Score,
            Comment = dto.Comment
        };

        dbContext.Ratings.Add(rating);
        await dbContext.SaveChangesAsync();

        return Result<RatingDto>.Success(new RatingDto(rating.Id, rating.ShipmentId, rating.Score, rating.Comment));
    }
}