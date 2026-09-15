
namespace RiverLine.Api.Services.Shipments;

public interface IShipmentService
{
    Task<ShipmentOperationResult> UpdateStatusAsync(Guid shipmentId, Guid carrierId, ShipmentStatus newStatus);
    Task<ShipmentDto?> GetByIdAsync(Guid id);
}