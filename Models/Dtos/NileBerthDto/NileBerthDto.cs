namespace RiverLine.Api.Models.Dtos.NileBerthDto;

public record NileBerthDto(
    Guid Id,
    string Name,
    string ArabicName,
    string Governorate,
    decimal Latitude,
    decimal Longitude,
    string Type,
    string Axis,
    string CoordinateAccuracy
);