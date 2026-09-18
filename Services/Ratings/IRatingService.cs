namespace RiverLine.Api.Services.Ratings;

public interface IRatingService
{
    Task<Result<RatingDto>> CreateAsync(Guid cargoOwnerId, CreateRatingDto dto);
    Task<Result<List<RatingDto>>> GetCarrierRatingsAsync(Guid carrierId);
}