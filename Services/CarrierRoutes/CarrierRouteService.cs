namespace RiverLine.Api.Services.CarrierRoutes;

public class CarrierRouteService(ApplicationDbContext dbContext) : ICarrierRouteService
{
    public async Task<Result<List<CarrierRouteDto>>> GetMyRoutesAsync(Guid carrierId)
    {
        var routes = await dbContext.CarrierRoutes
            .Where(r => r.CarrierProfile.UserId == carrierId)
            .Select(r => new CarrierRouteDto(r.Id, r.Origin, r.Destination, r.IsActive))
            .ToListAsync();

        return Result<List<CarrierRouteDto>>.Success(routes);
    }

    public async Task<Result<CarrierRouteDto>> AddRouteAsync(Guid carrierId, CreateCarrierRouteDto dto)
    {
        if (dto.Origin.Trim().Equals(dto.Destination.Trim(), StringComparison.OrdinalIgnoreCase))
            return Result.Failure(OperationError.Forbidden, "Origin and destination cannot be the same.");

        var profile = await dbContext.CarrierProfiles
            .SingleOrDefaultAsync(p => p.UserId == carrierId);

        if (profile is null)
            return Result.Failure(OperationError.NotFound, "Carrier profile not found.");

        var exists = await dbContext.CarrierRoutes.AnyAsync(r =>
            r.CarrierProfileId == profile.Id &&
            r.Origin.ToLower() == dto.Origin.Trim().ToLower() &&
            r.Destination.ToLower() == dto.Destination.Trim().ToLower());
        if (exists)
            return Result.Failure(OperationError.Conflict, "You already have this route.");
        
        var route = new CarrierRoute
        {
            Id = Guid.NewGuid(),
            CarrierProfileId = profile.Id,
            Origin = dto.Origin,
            Destination = dto.Destination,
            IsActive = true
        };

        dbContext.CarrierRoutes.Add(route);
        await dbContext.SaveChangesAsync();

        return Result<CarrierRouteDto>.Success(
            new CarrierRouteDto(route.Id, route.Origin, route.Destination, route.IsActive));
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
            .Select(r => new { r.Origin, r.Destination })
            .ToListAsync();

        if (!routes.Any())
            return Result<List<ShipmentRequestDto>>.Success(new List<ShipmentRequestDto>());

        var requests = await dbContext.ShipmentRequests
            .Where(r => r.Status == ShipmentRequestStatus.Open &&
                        routes.Any(route =>
                            route.Origin == r.Origin &&
                            route.Destination == r.Destination))
            .Select(r => 
                new ShipmentRequestDto(
                r.Id, 
                r.CargoType,
                r.Weight, 
                r.Origin, 
                r.Destination,
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