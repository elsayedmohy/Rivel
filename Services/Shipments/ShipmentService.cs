namespace RiverLine.Api.Services.Shipments;

public class ShipmentService(ApplicationDbContext dbContext,
    ShipmentMapper shipmentMapper,
    NileBerthMapper nileBerthMapper
    ) : IShipmentService
{
    
    private static readonly Dictionary<ShipmentStatus, ShipmentStatus> AllowedTransitions = new()
    {
        [ShipmentStatus.Matched] = ShipmentStatus.PickedUp,
        [ShipmentStatus.PickedUp] = ShipmentStatus.InTransit,
        [ShipmentStatus.InTransit] = ShipmentStatus.Delivered,
    };
    
    public async Task<Result<ShipmentDto>> UpdateStatusAsync(
        Guid shipmentId,
        Guid carrierId,
        ShipmentStatus newStatus)
    {
        var shipment = await ShipmentsWithDetails()
            .FirstOrDefaultAsync(s => s.Id == shipmentId);

        if (shipment is null)
        {
            return Result.Failure(
                OperationError.NotFound,
                "Shipment not found.");
        }

        var vesselOwnerId = await dbContext.CarrierProfiles
            .Where(cp => cp.Id == shipment.Vessel.CarrierProfileId)
            .Select(cp => cp.UserId)
            .FirstOrDefaultAsync();

        if (vesselOwnerId != carrierId)
        {
            return Result.Failure(
                OperationError.Forbidden,
                "You are not allowed to update this shipment.");
        }

        if (!AllowedTransitions.TryGetValue(
                shipment.Status,
                out var expectedNext) ||
            expectedNext != newStatus)
        {
            return Result.Failure(
                OperationError.Conflict,
                $"Cannot transition from {shipment.Status} to {newStatus}.");
        }

        shipment.Status = newStatus;

        if (newStatus == ShipmentStatus.Delivered)
        {
            var vessel = await dbContext.Vessels
                .FirstOrDefaultAsync(v => v.Id == shipment.VesselId);
            if (vessel is null)
            {
                return Result.Failure(
                    OperationError.NotFound,
                    "Vessel not found.");
            }

            vessel.Status = VesselStatus.Available;
        }

        await dbContext.SaveChangesAsync();

        return Result<ShipmentDto>.Success(shipmentMapper.ToDto(shipment));
    }
    
    
    public async Task<Result<List<ShipmentDto>>> GetAllAsync(Guid userId, bool? rated = null)
    {
        var query = dbContext.Shipments
            .AsNoTracking()
            .Where(x => x.ShipmentRequest.CargoOwnerId == userId
                        || x.Vessel.CarrierProfile.UserId == userId);

        if (rated == false)
            query = query.Where(s => s.Status == ShipmentStatus.Delivered && s.Rating == null);
        else if (rated == true)
            query = query.Where(s => s.Rating != null);

        var items = await query
            .OrderByDescending(s => s.ShipmentRequest.RequestedDate)
            .ThenBy(s => s.Id)
            .Select(s => new ShipmentDto(
                s.Id,
                s.Status.ToString(),
                s.ShipmentRequestId,
                s.ShipmentRequest.CargoType,
                s.ShipmentRequest.Weight,
                nileBerthMapper.ToBerthDto(s.ShipmentRequest.OriginNileBerth),
                nileBerthMapper.ToBerthDto(s.ShipmentRequest.DestinationNileBerth),
                s.ShipmentRequest.RequestedDate,
                s.ShipmentRequest.CargoOwnerId,
                s.ShipmentRequest.CargoOwner.Name,
                s.OfferId,
                s.Offer.Price,
                s.Offer.ProposedPickupDate,
                s.VesselId,
                s.Vessel.Type,
                s.Vessel.CarrierProfile.CompanyName,
                s.Rating != null,
                s.Rating == null ? null : new ShipmentRatingDto(
                    s.Rating.Score,
                    s.Rating.Comment,
                    s.Rating.CreatedAt)
            ))
            .ToListAsync();

        return Result<List<ShipmentDto>>.Success(items);
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