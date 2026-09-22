namespace RiverLine.Api.Models.Dtos.Rating;

public record ReceivedRatingDto(
    Guid Id,
    int Score,
    string? Comment,
    DateTime CreatedAt,
    string CargoType,
    string Origin,
    string Destination,
    string CargoOwnerName
    );
 
public record CarrierRatingsDto(
    decimal OverallRating,
    int RatingCount,
    IReadOnlyDictionary<int, int> Distribution,
    IReadOnlyList<ReceivedRatingDto> Items,
    int Page,
    int PageSize);
 
public record CarrierRatingsQuery(int Page = 1, int PageSize = 10);