namespace RiverLine.Api.Services.Ratings;

public interface IRatingService
{
    Task<Result<RatingDto>> CreateAsync(Guid shipmentId, Guid cargoOwnerId, CreateRatingDto dto);
}