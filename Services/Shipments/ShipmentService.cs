namespace RiverLine.Api.Services.Shipments;

public class ShipmentService(ApplicationDbContext dbContext,
    ShipmentMapper shipmentMapper) : IShipmentService
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
        var shipment = await ShipmentsWithDetails()
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

        return ShipmentOperationResult.Success(shipmentMapper.ToDto(shipment));
    }
    
    
    public async Task<Result<List<ShipmentDto>>> GetAllAsync(Guid userId)
    {
        var shipments = await dbContext.Shipments
            .Where(x => x.ShipmentRequest.CargoOwnerId == userId
                        || x.Vessel.CarrierProfile.UserId == userId)
            .Select(s => new ShipmentDto(
                s.Id,
                s.Status.ToString(),
                s.ShipmentRequestId,
                s.ShipmentRequest.CargoType,
                s.ShipmentRequest.Weight,
                s.ShipmentRequest.Origin,
                s.ShipmentRequest.Destination,
                s.ShipmentRequest.RequestedDate,
                s.ShipmentRequest.CargoOwnerId,
                s.ShipmentRequest.CargoOwner.Name,
                s.OfferId,
                s.Offer.Price,
                s.Offer.ProposedPickupDate,
                s.VesselId,
                s.Vessel.Type,
                s.Vessel.CarrierProfile.CompanyName,
                s.Rating == null ? null : new RatingDto(
                    s.Rating.Id,
                    s.Rating.ShipmentId,
                    s.Rating.Score,
                    s.Rating.Comment)
            ))
            .ToListAsync();

        return Result<List<ShipmentDto>>.Success(shipments);
    }

    public async Task<ShipmentDto?> GetByIdAsync(Guid id)
    {
        var shipment = await ShipmentsWithDetails().FirstOrDefaultAsync(s => s.Id == id);
        return shipment is null ? null : (shipmentMapper.ToDto(shipment));
    }

    private IQueryable<Shipment> ShipmentsWithDetails() =>
        dbContext.Shipments
            .Include(s => s.ShipmentRequest)
                .ThenInclude(sr => sr.CargoOwner)
            .Include(s => s.Offer)
            .Include(s => s.Vessel)
                .ThenInclude(v => v.CarrierProfile)
            .Include(s => s.Rating);

    public async Task<Result<RatingDto>> GetRatingAsync(Guid shipmentId, Guid requestingUserId)
    {
        var rating = await dbContext.Ratings
            .Where(r => r.ShipmentId == shipmentId)
            .Select(r => new RatingDto(r.Id, r.ShipmentId, r.Score, r.Comment))
            .SingleOrDefaultAsync();

        if (rating is null)
            return Result.Failure(OperationError.NotFound, "Rating not found.");

        return Result<RatingDto>.Success(rating);
    }
    
}