
namespace RiverLine.Api.Services.Vessels;

public interface IVesselService
{
    Task<Result<VesselDto>> CreateAsync(Guid carrierId, CreateVesselDto dto);
    Task<Result<List<VesselDto>>> GetMineAsync(Guid carrierId);
}
