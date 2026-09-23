namespace RiverLine.Api.Models.Dtos.ShipmentRequest;

public record SuggestedRequestDto(
    Guid Id,
    string CargoType,
    decimal Weight,
    DateOnly RequestedDate,
    int OffersCount,
    decimal? LowestOfferPrice,
    bool IsNew,
    NileBerthDto.NileBerthDto OriginNileBerth,
    NileBerthDto.NileBerthDto DestinationNileBerth,
    int FittingVesselsCount,
    decimal MaxVesselCapacity
    );
    
    
    
 
public record SuggestedRequestsPageDto(
    IReadOnlyList<SuggestedRequestDto> Items,
    int TotalCount,
    int Page,
    int PageSize,
    IReadOnlyList<RouteFilterOptionDto> RouteFilters);