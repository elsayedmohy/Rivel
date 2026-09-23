namespace RiverLine.Api.Services.Vessels;

public interface IVesselService
{
    Task<Result<VesselDto>> CreateAsync(Guid carrierId, CreateVesselDto dto);
    Task<Result<IReadOnlyList<VesselDto>>> GetMineAsync(Guid carrierId);

    Task<Result<VesselDto>> UpdateAsync(
        Guid carrierUserId, Guid vesselId, UpdateVesselDto request);

    Task<Result<VesselDto>> SetStatusAsync(
        Guid carrierUserId, Guid vesselId, VesselStatus status);

    Task<Result<bool>> ArchiveAsync(Guid carrierId, Guid vesselId);
    Task<Result<VesselDto>> GetOneAsync(Guid carrierUserId, Guid vesselId);
}