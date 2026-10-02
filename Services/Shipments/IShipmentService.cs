
namespace RiverLine.Api.Services.Shipments;

public interface IShipmentService
{

    Task<Result<List<ShipmentDto>>> GetAllAsync( Guid userId, bool? rated = null);
    Task<Result<ShipmentDto>> UpdateStatusAsync(Guid shipmentId, Guid carrierId, ShipmentStatus newStatus);
    Task<ShipmentDto?> GetByIdAsync(Guid id,Guid userId);
    Task<Result<ShipmentContactDto>> GetContactAsync(Guid shipmentId, Guid userId);
}