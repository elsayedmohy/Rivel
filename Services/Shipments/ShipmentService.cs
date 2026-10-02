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
        var query = VisibleTo(userId).AsNoTracking();

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
    public async Task<ShipmentDto?> GetByIdAsync(Guid id,Guid userId)
    {
        var shipment = await ShipmentsWithDetails()
            .Where(s => s.ShipmentRequest.CargoOwnerId == userId ||
                        s.Vessel.CarrierProfile.UserId == userId)
            .FirstOrDefaultAsync(s => s.Id == id);
        return shipment is null ? null : (shipmentMapper.ToDto(shipment));
    }
    
    
    public async Task<Result<ShipmentContactDto>> GetContactAsync(Guid shipmentId, Guid userId)
    {
        // parties to the Shipment Request  (طرفي الاتفاق)
        
        var parties = await VisibleTo(userId)
            .Where(s => s.Id == shipmentId)
            .Select(s => new
            {
                OwnerId = s.ShipmentRequest.CargoOwnerId,
                CarrierId = s.Vessel.CarrierProfile.UserId
            })
            .FirstOrDefaultAsync();

        if (parties is null)
            return Result.Failure(OperationError.NotFound, "shipment.not_found");

        var counterpartId = parties.OwnerId == userId ? parties.CarrierId : parties.OwnerId;

        var contact = await dbContext.Users
            .AsNoTracking()
            .Where(u => u.Id == counterpartId)
            .Select(u => new ShipmentContactDto(
                u.Name,
                u.CarrierProfile != null ? u.CarrierProfile.CompanyName : null,
                u.Email!,
                u.PhoneNumber))
            .FirstOrDefaultAsync();

        return contact is null
            ? Result.Failure(OperationError.NotFound, "shipment.not_found")
            : Result<ShipmentContactDto>.Success(contact);
    }

    private IQueryable<Shipment> ShipmentsWithDetails() =>
        dbContext.Shipments
            .Include(s => s.ShipmentRequest)
                .ThenInclude(sr => sr.CargoOwner)
            .Include(s => s.Offer)
            .Include(s => s.Vessel)
                .ThenInclude(v => v.CarrierProfile)
            .Include(s => s.Rating);
    
    
    
    private IQueryable<Shipment> VisibleTo(Guid userId) =>
        dbContext.Shipments.Where(s =>
            s.ShipmentRequest.CargoOwnerId == userId ||
            s.Vessel.CarrierProfile.UserId == userId);
    
}