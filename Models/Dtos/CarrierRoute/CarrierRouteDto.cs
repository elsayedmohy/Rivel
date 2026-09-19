namespace RiverLine.Api.Models.Dtos.CarrierRoute;

public record CarrierRouteDto(Guid Id, string Origin, string Destination, bool IsActive);
