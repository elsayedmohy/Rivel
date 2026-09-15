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
        return await dbContext.ShipmentRequests
            .Where(x => x.Status == ShipmentRequestStatus.Open)
            .Select(x => new ShipmentRequestDto(
                x.Id,
                x.CargoType,
                x.Weight,
                x.Origin,
                x.Destination,
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
                x.Origin,
                x.Destination,
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
                x.Origin,
                x.Destination,
                x.RequestedDate,
                x.Status.ToString(),
                x.CargoOwnerId,
                x.Offers.Count))
            .SingleOrDefaultAsync();
    }
}