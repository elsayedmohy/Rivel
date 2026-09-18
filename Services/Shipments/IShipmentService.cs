
namespace RiverLine.Api.Services.Shipments;

public interface IShipmentService
{

    Task<Result<List<ShipmentDto>>> GetAllAsync(Guid userId);
    Task<ShipmentOperationResult> UpdateStatusAsync(Guid shipmentId, Guid carrierId, ShipmentStatus newStatus);
    Task<ShipmentDto?> GetByIdAsync(Guid id);
}