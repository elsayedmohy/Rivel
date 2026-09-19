namespace RiverLine.Api.Services.CarrierRoutes;

public class CarrierRouteService(ApplicationDbContext dbContext) : ICarrierRouteService
{
    public async Task<Result<List<CarrierRouteDto>>> GetMyRoutesAsync(Guid carrierId)
    {
        var routes = await dbContext.CarrierRoutes
            .Where(r => r.CarrierProfile.UserId == carrierId)
            .Select(r => new CarrierRouteDto(
                r.Id,
                r.OriginNileBerth,
                r.DestinationNileBerth,
                r.IsActive))
            .ToListAsync();

        return Result<List<CarrierRouteDto>>.Success(routes);
    }

    public async Task<Result<CarrierRouteDto>> AddRouteAsync(Guid carrierId, CreateCarrierRouteDto dto)
    {
        if (dto.OriginNileBerthId == dto.DestinationNileBerthId)
            return Result.Failure(OperationError.BadRequest, "Origin and destination cannot be the same.");

        var profile = await dbContext.CarrierProfiles
            .SingleOrDefaultAsync(p => p.UserId == carrierId);

        if (profile is null)
            return Result.Failure(OperationError.NotFound, "Carrier profile not found.");

        var berths = await dbContext.NileBerths
            .Where(x => x.Id == dto.OriginNileBerthId || x.Id == dto.DestinationNileBerthId)
            .ToListAsync();

        var originNileBerth = berths.FirstOrDefault(x => x.Id == dto.OriginNileBerthId);
        if (originNileBerth is null)
            return Result.Failure(OperationError.NotFound, "Origin Nile Berth not found.");

        var destinationNileBerth = berths.FirstOrDefault(x => x.Id == dto.DestinationNileBerthId);
        if (destinationNileBerth is null)
            return Result.Failure(OperationError.NotFound, "Destination Nile Berth not found.");

        var exists = await dbContext.CarrierRoutes.AnyAsync(r =>
            r.CarrierProfileId == profile.Id &&
            r.OriginBerthId == dto.OriginNileBerthId &&
            r.DestinationBerthId == dto.DestinationNileBerthId);
        if (exists)
            return Result.Failure(OperationError.Conflict, "You already have this route.");
        
        var route = new CarrierRoute
        {
            Id = Guid.NewGuid(),
            CarrierProfileId = profile.Id,
            OriginNileBerth = originNileBerth,
            DestinationNileBerth = destinationNileBerth,
            IsActive = true
        };

        dbContext.CarrierRoutes.Add(route);
        await dbContext.SaveChangesAsync();

        return Result<CarrierRouteDto>.Success(
            new CarrierRouteDto(route.Id, route.OriginNileBerth, route.DestinationNileBerth, route.IsActive));
    }

    public async Task<Result<bool>> DeleteRouteAsync(Guid routeId, Guid carrierId)
    {
        var route = await dbContext.CarrierRoutes
            .Include(r => r.CarrierProfile)
            .SingleOrDefaultAsync(r => r.Id == routeId);

        if (route is null)
            return Result.Failure(OperationError.NotFound, "Route not found.");

        if (route.CarrierProfile.UserId != carrierId)
            return Result.Failure(OperationError.Forbidden, "Not your route.");

        dbContext.CarrierRoutes.Remove(route);
        await dbContext.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<List<ShipmentRequestDto>>> GetSuggestedRequestsAsync(Guid carrierId)
    {
        var routes = await dbContext.CarrierRoutes
            .Where(r => r.CarrierProfile.UserId == carrierId && r.IsActive)
            .Select(r => new { OriginPort = r.OriginNileBerth, DestinationPort = r.DestinationNileBerth })
            .ToListAsync();

        if (!routes.Any())
            return Result<List<ShipmentRequestDto>>.Success(new List<ShipmentRequestDto>());

        var requests = await dbContext.ShipmentRequests
            .Where(r => r.Status == ShipmentRequestStatus.Open &&
                        routes.Any(route =>
                            route.OriginPort == r.OriginNileBerth &&
                            route.DestinationPort == r.DestinationNileBerth))
            .Select(r => 
                new ShipmentRequestDto(
                r.Id, 
                r.CargoType,
                r.Weight, 
                r.OriginNileBerth, 
                r.DestinationNileBerth,
                r.RequestedDate,
                r.Status.ToString(),
                r.CargoOwnerId,
                r.Offers.Count
                )
            )
            .ToListAsync();

        return Result<List<ShipmentRequestDto>>.Success(requests);
    }
}