namespace RiverLine.Api.Services.Shipments;

public class ShipmentService(ApplicationDbContext dbContext) : IShipmentService
{
    
    private static readonly Dictionary<ShipmentStatus, ShipmentStatus> AllowedTransitions = new()
    {
        [ShipmentStatus.Matched] = ShipmentStatus.PickedUp,
        [ShipmentStatus.PickedUp] = ShipmentStatus.InTransit,
        [ShipmentStatus.InTransit] = ShipmentStatus.Delivered,
    };
    
    public async Task<ShipmentOperationResult> UpdateStatusAsync(
        Guid shipmentId,
        Guid carrierId,
        ShipmentStatus newStatus)
    {
        var shipment = await dbContext.Shipments
            .Include(s => s.Vessel)
            .FirstOrDefaultAsync(s => s.Id == shipmentId);

        if (shipment is null)
        {
            return ShipmentOperationResult.Failure(
                ShipmentOperationError.NotFound,
                "Shipment not found.");
        }

        var vesselOwnerId = await dbContext.CarrierProfiles
            .Where(cp => cp.Id == shipment.Vessel.CarrierProfileId)
            .Select(cp => cp.UserId)
            .FirstOrDefaultAsync();

        if (vesselOwnerId != carrierId)
        {
            return ShipmentOperationResult.Failure(
                ShipmentOperationError.Forbidden,
                "You are not allowed to update this shipment.");
        }

        if (!AllowedTransitions.TryGetValue(
                shipment.Status,
                out var expectedNext) ||
            expectedNext != newStatus)
        {
            return ShipmentOperationResult.Failure(
                ShipmentOperationError.Conflict,
                $"Cannot transition from {shipment.Status} to {newStatus}.");
        }

        shipment.Status = newStatus;

        await dbContext.SaveChangesAsync();

        return ShipmentOperationResult.Success(ToDto(shipment));
    }

    public async Task<ShipmentDto?> GetByIdAsync(Guid id)
    {
        var shipment = await dbContext.Shipments.FirstOrDefaultAsync(s => s.Id == id);
        return shipment is null ? null : ToDto(shipment);
    }
    
    private static ShipmentDto ToDto(Shipment s) =>
        new(s.Id, s.ShipmentRequestId, s.OfferId, s.VesselId, s.Status.ToString());
}