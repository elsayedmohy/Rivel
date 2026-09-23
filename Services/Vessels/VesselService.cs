namespace RiverLine.Api.Services.Vessels;

public class VesselService(ApplicationDbContext dbContext) : IVesselService
{
    private static readonly ShipmentStatus[] ActiveStatuses =
    [
        ShipmentStatus.Matched,
        ShipmentStatus.PickedUp,
        ShipmentStatus.InTransit
    ];

    public async Task<Result<VesselDto>> CreateAsync(Guid carrierId, CreateVesselDto request)
    {
        var profileId = await dbContext.CarrierProfiles
            .Where(cp => cp.UserId == carrierId)
            .Select(p => (Guid?)p.Id)
            .FirstOrDefaultAsync();

        if (profileId is null)
            return Result<VesselDto>.Failure(OperationError.NotFound,
                "Carrier profile not found. Please complete your profile first.");

        var registration = request.RegistrationNumber.Trim();

        var taken = await dbContext.Vessels
            .AnyAsync(v => v.RegistrationNumber == registration);

        if (taken)
            return Result<VesselDto>.Failure(OperationError.Conflict,
                "Registration Number is already exists.");

        var vessel = new Vessel
        {
            Id = Guid.NewGuid(),
            CarrierProfileId = profileId.Value,
            Name = request.Name.Trim(),
            RegistrationNumber = registration,
            Type = request.Type,
            Capacity = request.Capacity,
            Status = VesselStatus.Available,
            YearBuilt = request.YearBuilt,
            IsArchived = false
        };

        dbContext.Vessels.Add(vessel);
        await dbContext.SaveChangesAsync();

        return Result<VesselDto>.Success(ToDto(vessel, activeShipment: null, pendingOffers: 0));
    }

    public async Task<Result<IReadOnlyList<VesselDto>>> GetMineAsync(Guid carrierId)
    {
        var vessels = await dbContext.Vessels
            .AsNoTracking()
            .Where(v => v.CarrierProfile.UserId == carrierId && !v.IsArchived)
            .OrderBy(v => v.Name)
            .ThenBy(v => v.Id)
            .Select(v => new VesselDto(
                v.Id,
                v.Name,
                v.RegistrationNumber,
                v.Type,
                v.Capacity,
                v.Status,
                v.YearBuilt,
                v.Shipments
                    .Where(s => ActiveStatuses.Contains(s.Status))
                    .Select(s => new VesselAssignmentDto(
                        s.Id,
                        s.ShipmentRequest.CargoType,
                        s.ShipmentRequest.OriginNileBerth.ArabicName,
                        s.ShipmentRequest.DestinationNileBerth.ArabicName,
                        s.Status,
                        s.ShipmentRequest.RequestedDate))
                    .FirstOrDefault(),
                v.Offers.Count(o => o.Status == OfferStatus.Pending)))
            .ToListAsync();

        return Result<IReadOnlyList<VesselDto>>.Success(vessels);
    }

    public async Task<Result<VesselDto>> UpdateAsync(Guid carrierUserId, Guid vesselId, UpdateVesselDto request)
    {

        var vessel = await dbContext.Vessels
            .Include(v => v.CarrierProfile)
            .FirstOrDefaultAsync(v => v.Id == vesselId && !v.IsArchived);
        if (vessel is null)
            return Result.Failure(OperationError.NotFound, "Vessel not found.");
        if (vessel.CarrierProfile.UserId != carrierUserId)
            return Result.Failure(OperationError.Conflict, "Carrier doesn't have access to the requested vessel.");

        var registration = request.RegistrationNumber.Trim();
 
        if (registration != vessel.RegistrationNumber)
        {
            var taken = await dbContext.Vessels
                .AnyAsync(v => v.Id != vesselId && v.RegistrationNumber == registration);
 
            if (taken)
                return Result<VesselDto>.Failure(OperationError.Conflict,
                    "Registration Number is already exists.");
        }
 
        if (request.Capacity < vessel.Capacity)
        {
            var heaviestShipment = await dbContext.Shipments
                .Where(s => s.VesselId == vesselId && ActiveStatuses.Contains(s.Status))
                .MaxAsync(s => (decimal?)s.ShipmentRequest.Weight) ?? 0m;
 
            var heaviestOffer = await dbContext.Offers
                .Where(o => o.VesselId == vesselId && o.Status == OfferStatus.Pending)
                .MaxAsync(o => (decimal?)o.ShipmentRequest.Weight) ?? 0m;
 
            var committed = Math.Max(heaviestShipment, heaviestOffer);
 
            if (request.Capacity < committed)
                return Result<VesselDto>.Failure(
                    OperationError.Conflict,"Vessel Capacity is less than the commitment capacity.");
        }
 
        vessel.Name = request.Name.Trim();
        vessel.RegistrationNumber = registration;
        vessel.Type = request.Type;
        vessel.Capacity = request.Capacity;
        vessel.YearBuilt = request.YearBuilt;
 
        await dbContext.SaveChangesAsync();
 
        return await GetOneAsync(carrierUserId, vesselId);
    }

    public async Task<Result<VesselDto>> SetStatusAsync(
        Guid carrierUserId, Guid vesselId, VesselStatus status)
    {
        if (status is not (VesselStatus.Available or VesselStatus.Maintenance))
            return Result<VesselDto>.Failure(
                OperationError.Validation,"This status is configured from the system");
 
        var vessel = await dbContext.Vessels
            .Include(v => v.CarrierProfile)
            .FirstOrDefaultAsync(v => v.Id == vesselId && !v.IsArchived);
 
        if (vessel is null)
            return Result<VesselDto>.Failure(OperationError.NotFound, "vessel not found");
 
        if (vessel.CarrierProfile.UserId != carrierUserId)
            return Result<VesselDto>.Failure(OperationError.Forbidden, "vessel not yours");
 
        if (vessel.Status == VesselStatus.OnTrip)
            return Result<VesselDto>.Failure(OperationError.Conflict,"vessel on trip");
 
        if (status == VesselStatus.Maintenance)
        {
            var pending = await dbContext.Offers
                .Where(o => o.VesselId == vesselId && o.Status == OfferStatus.Pending)
                .ToListAsync();
 
            foreach (var offer in pending)
                offer.Status = OfferStatus.Withdrawn;
        }
 
        vessel.Status = status;
        await dbContext.SaveChangesAsync();
 
        return await GetOneAsync(carrierUserId, vesselId);
    }
 
    public async Task<Result<bool>> ArchiveAsync(Guid carrierUserId, Guid vesselId)
    {
        var vessel = await dbContext.Vessels
            .Include(v => v.CarrierProfile)
            .FirstOrDefaultAsync(v => v.Id == vesselId && !v.IsArchived);
 
        if (vessel is null)
            return Result.Failure(OperationError.NotFound,"vessel not found" );
 
        if (vessel.CarrierProfile.UserId != carrierUserId)
            return Result.Failure(OperationError.Forbidden,"vessel not yours" );
 
        var busy = await dbContext.Shipments
            .AnyAsync(s => s.VesselId == vesselId && ActiveStatuses.Contains(s.Status));
 
        if (busy)
            return Result.Failure(OperationError.Conflict,"vessel has active shipment");
 
        var pending = await dbContext.Offers
            .Where(o => o.VesselId == vesselId && o.Status == OfferStatus.Pending)
            .ToListAsync();
 
        foreach (var offer in pending)
            offer.Status = OfferStatus.Withdrawn;
 
        vessel.IsArchived = true;
        vessel.Status = VesselStatus.Maintenance;
 
        await dbContext.SaveChangesAsync();
        return Result<bool>.Success(true);
    }
 
    public async Task<Result<VesselDto>> GetOneAsync(Guid carrierUserId, Guid vesselId)
    {
        var dto = await dbContext.Vessels
            .AsNoTracking()
            .Where(v => v.Id == vesselId && v.CarrierProfile.UserId == carrierUserId)
            .Select(v => new VesselDto(
                v.Id, v.Name, v.RegistrationNumber, v.Type, v.Capacity, v.Status, v.YearBuilt,
                v.Shipments
                    .Where(s => ActiveStatuses.Contains(s.Status))
                    .Select(s => new VesselAssignmentDto(
                        s.Id,
                        s.ShipmentRequest.CargoType,
                        s.ShipmentRequest.OriginNileBerth.ArabicName,
                        s.ShipmentRequest.DestinationNileBerth.ArabicName,
                        s.Status,
                        s.ShipmentRequest.RequestedDate))
                    .FirstOrDefault(),
                v.Offers.Count(o => o.Status == OfferStatus.Pending)))
            .FirstOrDefaultAsync();
 
        return dto is null
            ? Result<VesselDto>.Failure(OperationError.NotFound,"vessel not found")
            : Result<VesselDto>.Success(dto);
    }


    private static VesselDto ToDto(Vessel v, VesselAssignmentDto? activeShipment, int pendingOffers)
        => new(v.Id, v.Name, v.RegistrationNumber, v.Type, v.Capacity,
            v.Status, v.YearBuilt, activeShipment, pendingOffers);
}