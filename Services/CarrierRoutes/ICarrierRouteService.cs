namespace RiverLine.Api.Services.CarrierRoutes;

public interface ICarrierRouteService
{
    Task<Result<List<CarrierRouteDto>>> GetMyRoutesAsync(Guid carrierId);
    Task<Result<CarrierRouteDto>> AddRouteAsync(Guid carrierId, CreateCarrierRouteDto dto);
    Task<Result<bool>> DeleteRouteAsync(Guid routeId, Guid carrierId);
    Task<Result<List<ShipmentRequestDto>>> GetSuggestedRequestsAsync(Guid carrierId);
}