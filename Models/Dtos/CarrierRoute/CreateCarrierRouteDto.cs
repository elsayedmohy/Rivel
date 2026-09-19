namespace RiverLine.Api.Models.Dtos.CarrierRoute;

public record CreateCarrierRouteDto(
    Guid OriginNileBerthId,
    Guid DestinationNileBerthId
);