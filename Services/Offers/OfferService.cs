using RiverLine.Api.Configurations;

namespace RiverLine.Api.Services.Offers;

public class OfferService(
    ApplicationDbContext dbContext,
    INotificationService notifications,
    IOptions<AppUrls> urls,
    ILogger<OfferService> logger,
    OfferMapper mapper,
    ShipmentMapper shipmentMapper
    ) : IOfferService
{
    
    private readonly AppUrls _urls = urls.Value;
    public async Task<Result<OfferDto>> CreateAsync(Guid carrierId, Guid shipmentRequestId, CreateOfferDto dto)
    {
        var request = await dbContext.ShipmentRequests
            .Include(r => r.OriginNileBerth)
            .Include(r => r.DestinationNileBerth)
            .FirstOrDefaultAsync(r => r.Id == shipmentRequestId);
        if (request is null)
            return Result.Failure(
                OperationError.NotFound, "Shipment request not found.");

        if (request.CargoOwnerId == carrierId)
            return Result.Failure(
                OperationError.Invalid,
                "You cannot offer on your own shipment request.");

        if (request.Status != ShipmentRequestStatus.Open)
            return Result.Failure(
                OperationError.Conflict,
                "This shipment request is no longer open for offers.");

        var hasActiveOffer = await dbContext.Offers.AnyAsync(o =>
            o.ShipmentRequestId == shipmentRequestId &&
            o.CarrierId == carrierId &&
            (o.Status == OfferStatus.Pending || o.Status == OfferStatus.Accepted));
        if (hasActiveOffer)
            return Result.Failure(
                OperationError.Conflict,
                "You already have an active offer on this shipment request.");

        var vessel = await dbContext.Vessels
            .Include(v => v.CarrierProfile)
            .FirstOrDefaultAsync(v => v.Id == dto.VesselId);
        if (vessel is null)
            return Result.Failure(
                OperationError.Invalid, "Vessel not found.");

        if (vessel.CarrierProfile.UserId != carrierId)
            return Result.Failure(
                OperationError.Invalid, "You can only offer a vessel you own.");

        if (vessel.Status != VesselStatus.Available)
            return Result.Failure(
                OperationError.Invalid, "The chosen vessel is not available.");

        var carrier = await dbContext.Users
            .Include(u => u.CarrierProfile)
            .FirstAsync(u => u.Id == carrierId);

        if (request.Weight > vessel.Capacity)
            return Result.Failure(OperationError.BadRequest,
                $"Vessel capacity ({vessel.Capacity} tons) is less than cargo weight ({request.Weight} tons).");

        var offer = new Offer
        {
            Id = Guid.NewGuid(),
            ShipmentRequestId = shipmentRequestId,
            CarrierId = carrierId,
            VesselId = dto.VesselId,
            Price = dto.Price,
            ProposedPickupDate = dto.ProposedPickupDate,
            Status = OfferStatus.Pending,
            Carrier = carrier
        };

        dbContext.Offers.Add(offer);
        await dbContext.SaveChangesAsync();
        var cargoOwner = await dbContext.Users.FindAsync(request.CargoOwnerId);
        var originName = request.OriginNileBerth.Name;
        var destName = request.DestinationNileBerth.Name;

        await notifications.CreateAsync(new NotificationRequest(
            UserId:   request.CargoOwnerId,
            Type:     NotificationType.OfferReceived,
            EntityId: request.Id,
            Data: new
            {
                cargoType   = request.CargoType,
                origin      = request.OriginNileBerth.ArabicName,
                destination = request.DestinationNileBerth.ArabicName,
                price       = offer.Price,
                pickupDate  = offer.ProposedPickupDate,
                carrierName = vessel.CarrierProfile.CompanyName,
                actionUrl   = _urls.Request(request.Id)
            }));

        return Result<OfferDto>.Success(mapper.ToDto(offer));
    }

    public async Task<List<OfferDto>?> GetForRequestAsync(Guid shipmentRequestId, Guid viewerId)
    {
        var request = await dbContext.ShipmentRequests
            .Where(r => r.Id == shipmentRequestId)
            .Select(r => new { r.CargoOwnerId })
            .SingleOrDefaultAsync();
        if (request is null || request.CargoOwnerId != viewerId)
            return null;

        return await dbContext.Offers
            .Where(o => o.ShipmentRequestId == shipmentRequestId)
            .OrderBy(o => o.Price)
            .ThenBy(o => o.ProposedPickupDate)
            .Select(o => new OfferDto(
                o.Id,
                o.ShipmentRequestId,
                o.Shipment == null ? null : o.Shipment.Id,
                o.CarrierId,
                o.VesselId,
                o.Price,
                o.ProposedPickupDate,
                o.Status.ToString(),
                o.Carrier.Name,
                o.Carrier.CarrierProfile == null ? null : o.Carrier.CarrierProfile.CompanyName))
            .ToListAsync();
    }

    public async Task<List<OfferDto>> GetMineAsync(Guid carrierId)
    {
        return await dbContext.Offers
            .Where(o => o.CarrierId == carrierId)
            .OrderBy(o => o.ProposedPickupDate)
            .Select(o => new OfferDto(
                o.Id,
                o.ShipmentRequestId,
                o.Shipment == null ? null : o.Shipment.Id,
                o.CarrierId,
                o.VesselId,
                o.Price,
                o.ProposedPickupDate,
                o.Status.ToString(),
                o.Carrier.Name,
                o.Carrier.CarrierProfile == null ? null : o.Carrier.CarrierProfile.CompanyName))
            .ToListAsync();
    }

    public async Task<Result<ShipmentDto>> AcceptAsync(Guid cargoOwnerId, Guid offerId)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        var lockedOffer = await dbContext.Offers
            .FromSqlInterpolated($@"SELECT * FROM ""Offers"" WHERE ""Id"" = {offerId} FOR UPDATE")
            .FirstOrDefaultAsync();
 
        if (lockedOffer is null)
            return Result<ShipmentDto>.Failure(OperationError.NotFound,"offer not found");
        
        var offer = await dbContext.Offers
            .Include(o => o.Carrier)
            .ThenInclude(c => c.CarrierProfile)
            .Include(o => o.ShipmentRequest)
            .ThenInclude(r => r.OriginNileBerth)
            .Include(o => o.ShipmentRequest)
            .ThenInclude(r => r.DestinationNileBerth)
            .FirstOrDefaultAsync(o => o.Id == offerId);
        if (offer is null)
        {
            await transaction.RollbackAsync();
            return Result.Failure(OperationError.NotFound, "Offer not found.");
        }

        var request = await dbContext.ShipmentRequests
            .FromSqlInterpolated($"""
                                  SELECT *
                                  FROM "ShipmentRequests"
                                  WHERE "Id" = {offer.ShipmentRequestId}
                                  FOR UPDATE
                                  """)
            .SingleOrDefaultAsync();
        if (request is null || request.CargoOwnerId != cargoOwnerId)
        {
            await transaction.RollbackAsync();
            return Result.Failure(OperationError.NotFound, "Offer not found.");
        }

        if (request.Status != ShipmentRequestStatus.Open)
        {
            await transaction.RollbackAsync();
            return Result.Failure(OperationError.Conflict,
                "This shipment request is no longer open for offers.");
        }

        if (offer.Status != OfferStatus.Pending)
        {
            await transaction.RollbackAsync();
            return Result.Failure(
                OperationError.Conflict,
                "This offer is no longer pending.");
        }

        var vessel = await dbContext.Vessels
            .FromSqlInterpolated($"""
                                  SELECT *
                                  FROM "Vessels"
                                  WHERE "Id" = {offer.VesselId}
                                  FOR UPDATE
                                  """)
            .SingleOrDefaultAsync();
        if (vessel is null)
        {
            await transaction.RollbackAsync();
            return Result.Failure(
                OperationError.Invalid,
                "The offer's vessel no longer exists.");
        }
        
        if (vessel.IsArchived)
            return Result<ShipmentDto>.Failure(
                OperationError.Conflict,
                "vessel is archived");

        if (vessel.Status != VesselStatus.Available)
        {
            await transaction.RollbackAsync();
            return Result.Failure(OperationError.Conflict,
                "The offer's vessel is no longer available.");
        }
        
        if (vessel.Capacity < request.Weight)
            return Result<ShipmentDto>.Failure(OperationError.Conflict
                ,"vessel apacity insufficient");

        offer.Status = OfferStatus.Accepted;
        vessel.Status = VesselStatus.OnTrip;
        request.Status = ShipmentRequestStatus.Matched;
        
        var shipment = new Shipment
        {
            Id = Guid.NewGuid(),
            ShipmentRequestId = request.Id,
            OfferId = offer.Id,
            VesselId = vessel.Id,
            Status = ShipmentStatus.Matched
        };

        dbContext.Shipments.Add(shipment);
        
        var rejected = await dbContext.Offers
            .Where(o => o.ShipmentRequestId == offer.ShipmentRequestId && o.Id != offer.Id)
            .ToListAsync();
        
        foreach (var sibling in rejected.Where(s => s.Status == OfferStatus.Pending))
            sibling.Status = OfferStatus.Rejected;
        
        var rejectedIds = rejected.Select(o => o.Id).ToHashSet();
 
        var withdrawn = await dbContext.Offers
            .Include(o => o.ShipmentRequest)
            .Where(o => o.VesselId == vessel.Id
                        && o.Id != offer.Id
                        && o.Status == OfferStatus.Pending)
            .ToListAsync();
 
        withdrawn = withdrawn.Where(o => !rejectedIds.Contains(o.Id)).ToList();
 
        foreach (var other in withdrawn)
            other.Status = OfferStatus.Withdrawn;
        
        await dbContext.SaveChangesAsync();
        await transaction.CommitAsync();

        
       await NotifyAfterAcceptAsync(offer,request,shipment,vessel, rejected, withdrawn);
       
        return Result<ShipmentDto>.Success(shipmentMapper.ToDto(shipment));
    }
    
    
        private async Task NotifyAfterAcceptAsync(
        Offer accepted,
        ShipmentRequest request,
        Shipment shipment,
        Vessel vessel,
        IReadOnlyList<Offer> rejected,
        IReadOnlyList<Offer> withdrawn)
    {
        var origin = request.OriginNileBerth.ArabicName;
        var destination = request.DestinationNileBerth.ArabicName;
 
        var batch = new List<NotificationRequest>(rejected.Count + withdrawn.Count + 1)
        {
            new(accepted.CarrierId,
                NotificationType.OfferAccepted,
                shipment.Id,
                new
                {
                    price       = accepted.Price,
                    origin,
                    destination,
                    pickupDate  = accepted.ProposedPickupDate,
                    vesselName  = vessel.Name,
                    actionUrl   = _urls.Shipment(shipment.Id)
                })
        };
 
        batch.AddRange(rejected.Select(o => new NotificationRequest(
            o.CarrierId,
            NotificationType.OfferRejected,
            request.Id,
            new { cargoType = request.CargoType, origin, destination,
                  actionUrl = _urls.MyOffers() })));
 
        batch.AddRange(withdrawn.Select(o => new NotificationRequest(
            o.ShipmentRequest.CargoOwnerId,
            NotificationType.OfferWithdrawn,
            o.ShipmentRequestId,
            new { cargoType   = o.ShipmentRequest.CargoType,
                  carrierName = vessel.CarrierProfile.CompanyName,
                  actionUrl   = _urls.Request(o.ShipmentRequestId) })));
 
        try
        {
            await notifications.CreateManyAsync(batch);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Notifications failed after accepting offer {OfferId}", accepted.Id);
        }
    }
    
}