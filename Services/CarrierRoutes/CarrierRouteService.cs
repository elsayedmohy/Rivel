using System.Linq.Dynamic.Core;
using RiverLine.Api.Models.Dtos;

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
            r.OriginNileBerthId == dto.OriginNileBerthId &&
            r.DestinationNileBerthId == dto.DestinationNileBerthId);
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

    // public async Task<Result<SuggestedRequestsPageDto>> GetSuggestedRequestsAsync(Guid carrierId)
    // {
    //     var routes = dbContext.CarrierRoutes
    //         .Where(r => r.CarrierProfile.UserId == carrierId && r.IsActive)
    //         .Select(r => new
    //         {
    //             OriginPort = r.OriginNileBerth,
    //             DestinationPort = r.DestinationNileBerth
    //         });
    //
    //     // if (!routes.Any())
    //     //     return Result<List<ShipmentRequestDto>>.Success(new List<ShipmentRequestDto>());
    //
    //     var requests = await dbContext.ShipmentRequests
    //         .Where(r => r.Status == ShipmentRequestStatus.Open &&
    //                     routes.Any(route =>
    //                         route.OriginPort == r.OriginNileBerth &&
    //                         route.DestinationPort == r.DestinationNileBerth))
    //         .Select(r => 
    //             new ShipmentRequestDto(
    //             r.Id, 
    //             r.CargoType,
    //             r.Weight, 
    //             r.OriginNileBerth, 
    //             r.DestinationNileBerth,
    //             r.RequestedDate,
    //             r.Status.ToString(),
    //             r.CargoOwnerId,
    //             r.Offers.Count
    //             )
    //         )
    //         .ToListAsync();
    //
    //     return Result<List<ShipmentRequestDto>>.Success(requests);
    // }
    
    public async Task<Result<SuggestedRequestsPageDto>> GetSuggestedRequestsAsync(
    Guid carrierId, SuggestedRequestsQuery query)
{
    var page = Math.Max(1, query.Page);
    var pageSize = Math.Clamp(query.PageSize, 1, 50);
    var today = DateOnly.FromDateTime(DateTime.UtcNow);
    var newSince = DateTime.UtcNow.AddHours(-24);
 
    var myRoutes = dbContext.CarrierRoutes
        .Where(r => r.CarrierProfile.UserId == carrierId && r.IsActive);
 
    if (query.RouteId is { } routeId)
    {
        var owns = await myRoutes.AnyAsync(r => r.Id == routeId);
        if (!owns)
            return Result.Failure(OperationError.NotFound, "Route not found.");
    }
 
    var capacities = await dbContext.Vessels
        .AsNoTracking()
        .Where(v => v.CarrierProfile.UserId == carrierId && v.Status == VesselStatus.Available)
        .Select(v => v.Capacity)
        .ToListAsync();
 
    var maxCapacity = capacities.Count > 0 ? capacities.Max() : 0;
 
    var baseQuery = dbContext.ShipmentRequests
        .AsNoTracking()
        .Where(s => s.Status == ShipmentRequestStatus.Open)
        .Where(s => s.RequestedDate >= today)
        .Where(s => s.Offers.All(o => o.CarrierId != carrierId));
 
    if (query.FittingOnly)
        baseQuery = baseQuery.Where(s => s.Weight <= maxCapacity);
 
    var perRoute = await myRoutes
        .OrderBy(r => r.OriginNileBerth.ArabicName)
        .Select(r => new
        {
            r.Id,
            Origin = r.OriginNileBerth.ArabicName,
            Destination = r.DestinationNileBerth.ArabicName,
            Count = baseQuery.Count(s =>
                s.OriginNileBerthId == r.OriginNileBerthId &&
                s.DestinationNileBerthId == r.DestinationNileBerthId)
        })
        .ToListAsync();
 
    var routeFilters = new List<RouteFilterOptionDto>
    {
        new(null, "كل المسارات", perRoute.Sum(r => r.Count))
    };
    routeFilters.AddRange(perRoute
        .Where(r => r.Count > 0)
        .Select(r => new RouteFilterOptionDto(r.Id, $"{r.Origin} ← {r.Destination}", r.Count)));
 
    var matching = query.RouteId is { } selectedId
        ? baseQuery.Where(s => myRoutes.Any(r =>
            r.Id == selectedId &&
            r.OriginNileBerthId == s.OriginNileBerthId &&
            r.DestinationNileBerthId == s.DestinationNileBerthId))
        : baseQuery.Where(s => myRoutes.Any(r =>
            r.OriginNileBerthId == s.OriginNileBerthId &&
            r.DestinationNileBerthId == s.DestinationNileBerthId));
 
    var totalCount = await matching.CountAsync();
 
    matching = query.Sort switch
    {
        SuggestedSort.PickupSoonest => matching.OrderBy(s => s.RequestedDate).ThenBy(s => s.Id),
        SuggestedSort.WeightAsc     => matching.OrderBy(s => s.Weight).ThenBy(s => s.Id),
        SuggestedSort.WeightDesc    => matching.OrderByDescending(s => s.Weight).ThenBy(s => s.Id),
        _                           => matching.OrderByDescending(s => s.CreatedAt).ThenBy(s => s.Id),
    };
 
    var rows = await matching
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(s => new
        {
            s.Id,
            s.CargoType,
            s.Weight,
            s.RequestedDate,
            s.CreatedAt,
            OffersCount = s.Offers.Count(o => o.Status == OfferStatus.Pending),
            LowestOfferPrice = s.Offers
                .Where(o => o.Status == OfferStatus.Pending)
                .Min(o => (decimal?)o.Price),
            Origin = s.OriginNileBerth,
            Destination = s.DestinationNileBerth
        })
        .ToListAsync();
 
    var items = rows.Select(r => new SuggestedRequestDto(
        r.Id,
        r.CargoType,
        r.Weight,
        r.RequestedDate,
        r.OffersCount,
        r.LowestOfferPrice,
        IsNew: r.CreatedAt >= newSince,
        ToBerthDto(r.Origin),
        ToBerthDto(r.Destination),
        FittingVesselsCount: capacities.Count(c => c >= r.Weight),
        MaxVesselCapacity: maxCapacity
    )).ToList();
 
    return Result<SuggestedRequestsPageDto>.Success(
        new SuggestedRequestsPageDto(items, totalCount, page, pageSize, routeFilters));
}
    private static NileBerthDto ToBerthDto(NileBerth b) => new(
        b.Id, b.Name, b.ArabicName, b.Governorate, b.Latitude, b.Longitude,
        b.Type.ToString(), b.Axis.ToString(), b.CoordinateAccuracy.ToString());
    
}