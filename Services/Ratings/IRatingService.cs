namespace RiverLine.Api.Services.Ratings;

public interface IRatingService
{
    Task<Result<RatingDto>> CreateAsync(Guid cargoOwnerId, CreateRatingDto dto);

    Task<Result<CarrierRatingsDto>> GetMyRatingsAsync(
        Guid carrierId, CarrierRatingsQuery query);
}