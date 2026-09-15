namespace RiverLine.Api.Services.Offers;

public interface IOfferService
{
    Task<OfferOperationResult> CreateAsync(Guid carrierId, Guid shipmentRequestId, CreateOfferDto dto);
    Task<List<OfferDto>?> GetForRequestAsync(Guid shipmentRequestId, Guid viewerId);
    Task<List<OfferDto>> GetMineAsync(Guid carrierId);
    Task<OfferOperationResult> AcceptAsync(Guid cargoOwnerId, Guid offerId);
}