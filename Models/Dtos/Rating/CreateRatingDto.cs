namespace RiverLine.Api.Models.Dtos.Rating;

public record CreateRatingDto(
    Guid ShipmentId, 
    int Score, 
    string? Comment);