namespace RiverLine.Api.Services.Offers;

public class OfferService(
    ApplicationDbContext dbContext,
    OfferMapper mapper) : IOfferService
{
    public async Task<OfferOperationResult> CreateAsync(Guid carrierId, Guid shipmentRequestId, CreateOfferDto dto)
    {
        var request = await dbContext.ShipmentRequests
            .FirstOrDefaultAsync(r => r.Id == shipmentRequestId);
        if (request is null)
            return OfferOperationResult.Failure(
                OfferOperationError.NotFound, "Shipment request not found.");

        if (request.CargoOwnerId == carrierId)
            return OfferOperationResult.Failure(
                OfferOperationError.Invalid,
                "You cannot offer on your own shipment request.");

        if (request.Status != ShipmentRequestStatus.Open)
            return OfferOperationResult.Failure(
                OfferOperationError.Conflict,
                "This shipment request is no longer open for offers.");

        var hasActiveOffer = await dbContext.Offers.AnyAsync(o =>
            o.ShipmentRequestId == shipmentRequestId &&
            o.CarrierId == carrierId &&
            (o.Status == OfferStatus.Pending || o.Status == OfferStatus.Accepted));
        if (hasActiveOffer)
            return OfferOperationResult.Failure(
                OfferOperationError.Conflict,
                "You already have an active offer on this shipment request.");

        var vessel = await dbContext.Vessels
            .Include(v => v.CarrierProfile)
            .FirstOrDefaultAsync(v => v.Id == dto.VesselId);
        if (vessel is null)
            return OfferOperationResult.Failure(
                OfferOperationError.Invalid, "Vessel not found.");

        if (vessel.CarrierProfile.UserId != carrierId)
            return OfferOperationResult.Failure(
                OfferOperationError.Invalid, "You can only offer a vessel you own.");

        if (vessel.Status != VesselStatus.Available)
            return OfferOperationResult.Failure(
                OfferOperationError.Invalid, "The chosen vessel is not available.");

        var carrier = await dbContext.Users
            .Include(u => u.CarrierProfile)
            .FirstAsync(u => u.Id == carrierId);

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

        return OfferOperationResult.Success(mapper.ToDto(offer));
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
                o.CarrierId,
                o.VesselId,
                o.Price,
                o.ProposedPickupDate,
                o.Status.ToString(),
                o.Carrier.Name,
                o.Carrier.CarrierProfile == null ? null : o.Carrier.CarrierProfile.CompanyName))
            .ToListAsync();
    }

    public async Task<OfferOperationResult> AcceptAsync(Guid cargoOwnerId, Guid offerId)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        var offer = await dbContext.Offers
            .Include(o => o.Carrier)
            .ThenInclude(c => c.CarrierProfile)
            .FirstOrDefaultAsync(o => o.Id == offerId);
        if (offer is null)
        {
            await transaction.RollbackAsync();
            return OfferOperationResult.Failure(OfferOperationError.NotFound, "Offer not found.");
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
            return OfferOperationResult.Failure(OfferOperationError.NotFound, "Offer not found.");
        }

        if (request.Status != ShipmentRequestStatus.Open)
        {
            await transaction.RollbackAsync();
            return OfferOperationResult.Failure(OfferOperationError.Conflict,
                "This shipment request is no longer open for offers.");
        }

        if (offer.Status != OfferStatus.Pending)
        {
            await transaction.RollbackAsync();
            return OfferOperationResult.Failure(
                OfferOperationError.Conflict,
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
            return OfferOperationResult.Failure(
                OfferOperationError.Invalid,
                "The offer's vessel no longer exists.");
        }

        if (vessel.Status != VesselStatus.Available)
        {
            await transaction.RollbackAsync();
            return OfferOperationResult.Failure(OfferOperationError.Conflict,
                "The offer's vessel is no longer available.");
        }

        offer.Status = OfferStatus.Accepted;
        vessel.Status = VesselStatus.OnTrip;
        request.Status = ShipmentRequestStatus.Matched;

        var siblings = await dbContext.Offers
            .Where(o => o.ShipmentRequestId == offer.ShipmentRequestId && o.Id != offer.Id)
            .ToListAsync();
        foreach (var sibling in siblings.Where(s => s.Status == OfferStatus.Pending))
            sibling.Status = OfferStatus.Rejected;

        dbContext.Shipments.Add(new Shipment
        {
            Id = Guid.NewGuid(),
            ShipmentRequestId = request.Id,
            OfferId = offer.Id,
            VesselId = vessel.Id,
            Status = ShipmentStatus.Matched
        });

        await dbContext.SaveChangesAsync();
        await transaction.CommitAsync();

        return OfferOperationResult.Success(mapper.ToDto(offer));
    }
}