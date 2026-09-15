using RiverLine.Api.Mappers;

namespace RiverLine.Api.Services.ShipmentRequests;

public class ShipmentRequestService(
    ApplicationDbContext dbContext,
    ShipmentRequestMapper mapper) : IShipmentRequestService
{
    public async Task<ShipmentRequestDto> CreateAsync(Guid cargoOwnerId, CreateShipmentRequestDto dto)
    {
        var entity = new ShipmentRequest
        {
            Id = Guid.NewGuid(),
            CargoOwnerId = cargoOwnerId,
            CargoType = dto.CargoType,
            Weight = dto.Weight,
            Origin = dto.Origin,
            Destination = dto.Destination,
            RequestedDate = dto.RequestedDate,
            Status = ShipmentRequestStatus.Open
        };
        dbContext.ShipmentRequests.Add(entity);
        await dbContext.SaveChangesAsync();
        return mapper.ToDto(entity);
    }

    public async Task<List<ShipmentRequestDto>> GetOpenAsync()
    {
        var requests = await  dbContext.ShipmentRequests.Where(x=> 
            x.Status == ShipmentRequestStatus.Open)
            .Include(r => r.Offers)
            .Select(x=> mapper.ToDto(x))
            .ToListAsync();
        return requests;
    }

    public async Task<List<ShipmentRequestDto>> GetMineAsync(Guid cargoOwnerId)
    {
        var requests = await  dbContext.ShipmentRequests.Where(x=> 
                x.CargoOwnerId == cargoOwnerId)
            .Include(r => r.Offers)
            .Select(x=> mapper.ToDto(x))
            .ToListAsync();
        return requests;
    }

    public async Task<ShipmentRequestDto?> GetByIdAsync(Guid id)
    {
        var entity = await dbContext.ShipmentRequests
            .Include(r => r.Offers)
            .FirstOrDefaultAsync(r => r.Id == id);
        return entity is null ? null : mapper.ToDto(entity);
    }
}