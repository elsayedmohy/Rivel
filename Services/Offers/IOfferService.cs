namespace RiverLine.Api.Services.Offers;

public interface IOfferService
{
    Task<Result<OfferDto>>CreateAsync(Guid carrierId, Guid shipmentRequestId, CreateOfferDto dto);
    Task<List<OfferDto>?> GetForRequestAsync(Guid shipmentRequestId, Guid viewerId);
    Task<List<OfferDto>> GetMineAsync(Guid carrierId);
    Task<Result<OfferDto>> AcceptAsync(Guid cargoOwnerId, Guid offerId);
}