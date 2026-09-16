namespace RiverLine.Api.Models.Dtos.Rating;


public record RatingDto(Guid Id, Guid ShipmentId, int Score, string? Comment);