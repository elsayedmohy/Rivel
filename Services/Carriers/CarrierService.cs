namespace RiverLine.Api.Services.Carriers;

public class CarrierService(ApplicationDbContext dbContext) : ICarrierService
{
    public async Task<Result<CarrierPublicProfileDto>> GetPublicProfileAsync(Guid userId)
    {
        var profile = await dbContext.CarrierProfiles
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .Select(p => new CarrierPublicProfileDto(
                p.UserId,
                p.CompanyName,
                p.Bio,
                p.OverallRating,
                p.RatingCount,
                dbContext.Shipments.Count(s =>
                    s.Offer.CarrierId == p.UserId && s.Status == ShipmentStatus.Delivered),
                p.Vessels.Count(v => !v.IsArchived),
                p.Routes
                    .Where(r => r.IsActive)
                    .OrderBy(r => r.OriginNileBerth.ArabicName)
                    .Select(r => new CarrierRouteSummaryDto(
                        r.OriginNileBerth.ArabicName,
                        r.DestinationNileBerth.ArabicName))
                    .ToList()))
            .FirstOrDefaultAsync();

        if (profile is null)
            return Result.Failure(OperationError.NotFound, "carrier.not_found");

        return Result<CarrierPublicProfileDto>.Success(profile);
    }
}