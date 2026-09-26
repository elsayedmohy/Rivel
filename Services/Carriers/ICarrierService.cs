namespace RiverLine.Api.Services.Carriers;

public interface ICarrierService
{
    Task<Result<CarrierPublicProfileDto>> GetPublicProfileAsync(Guid userId);
}