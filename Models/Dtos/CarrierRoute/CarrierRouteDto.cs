namespace RiverLine.Api.Models.Dtos.CarrierRoute;

public record CarrierRouteDto(
    Guid Id,
    NileBerth OriginNileBerth,
    NileBerth DestinationNileBerth,
    bool IsActive
    );