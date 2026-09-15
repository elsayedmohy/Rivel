namespace RiverLine.Api.Services.ShipmentRequests;

public interface IShipmentRequestService
{
    Task<ShipmentRequestDto> CreateAsync(Guid cargoOwnerId, CreateShipmentRequestDto dto);
    Task<List<ShipmentRequestDto>> GetOpenAsync();
    Task<List<ShipmentRequestDto>> GetMineAsync(Guid cargoOwnerId);
    Task<ShipmentRequestDto?> GetByIdAsync(Guid id);
}