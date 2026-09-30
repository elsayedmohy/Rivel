namespace RiverLine.Api.Models.Dtos.Carrier;

public record CarrierRouteSummaryDto(string OriginName, string DestinationName);

public record CarrierPublicProfileDto(
    Guid UserId,
    string CompanyName,
    string? Bio,
    decimal OverallRating,
    int RatingCount,
    int CompletedShipments,
    int VesselCount,
    IReadOnlyList<CarrierRouteSummaryDto> ActiveRoutes,
    string? LogoPath);