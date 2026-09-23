namespace RiverLine.Api.Services.ShipmentRequests;

public class ShipmentRequestService(
    ApplicationDbContext dbContext,
    ShipmentRequestMapper mapper,
    MatchNotificationQueue matchNotificationQueue) : IShipmentRequestService
{
    public async Task<Result<ShipmentRequestDto>> CreateAsync(Guid cargoOwnerId, CreateShipmentRequestDto dto)
    {
        var berths = await dbContext.NileBerths
            .Where(x => x.Id == dto.OriginNileBerthId || x.Id == dto.DestinationNileBerthId)
            .ToListAsync();

        var originNileBerth = berths.FirstOrDefault(x => x.Id == dto.OriginNileBerthId);
        if (originNileBerth is null)
            return Result.Failure(OperationError.NotFound, "Origin Nile Berth not found.");

        var destinationNileBerth = berths.FirstOrDefault(x => x.Id == dto.DestinationNileBerthId);
        if (destinationNileBerth is null)
            return Result.Failure(OperationError.NotFound, "Destination Nile Berth not found.");

        var entity = new ShipmentRequest
        {
            Id = Guid.NewGuid(),
            CargoOwnerId = cargoOwnerId,
            CargoType = dto.CargoType,
            Weight = dto.Weight,
            OriginNileBerth = originNileBerth,
            DestinationNileBerth = destinationNileBerth,
            RequestedDate = dto.RequestedDate,
            Status = ShipmentRequestStatus.Open
        };
        dbContext.ShipmentRequests.Add(entity);
        await dbContext.SaveChangesAsync();
        await matchNotificationQueue.EnqueueAsync(entity.Id);
        return Result<ShipmentRequestDto>.Success(mapper.ToDto(entity));
    }

    public async Task<List<ShipmentRequestDto>> GetOpenAsync()
    {
        return await dbContext.ShipmentRequests
            .Where(x => x.Status == ShipmentRequestStatus.Open)
            .Select(x => new ShipmentRequestDto(
                x.Id,
                x.CargoType,
                x.Weight,
                x.OriginNileBerth,
                x.DestinationNileBerth,
                x.RequestedDate,
                x.Status.ToString(),
                x.CargoOwnerId,
                x.Offers.Count))
            .ToListAsync();
    }

    public async Task<List<ShipmentRequestDto>> GetMineAsync(Guid cargoOwnerId)
    {
        return await dbContext.ShipmentRequests
            .Where(x => x.CargoOwnerId == cargoOwnerId)
            .Select(x => new ShipmentRequestDto(
                x.Id,
                x.CargoType,
                x.Weight,
                x.OriginNileBerth,
                x.DestinationNileBerth,
                x.RequestedDate,
                x.Status.ToString(),
                x.CargoOwnerId,
                x.Offers.Count))
            .ToListAsync();
    }

    public async Task<ShipmentRequestDto?> GetByIdAsync(Guid id)
    {
        return await dbContext.ShipmentRequests
            .Where(x => x.Id == id)
            .Select(x => new ShipmentRequestDto(
                x.Id,
                x.CargoType,
                x.Weight,
                x.OriginNileBerth,
                x.DestinationNileBerth,
                x.RequestedDate,
                x.Status.ToString(),
                x.CargoOwnerId,
                x.Offers.Count))
            .SingleOrDefaultAsync();
    }
}