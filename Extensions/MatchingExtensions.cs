namespace RiverLine.Api.Extensions;

public static class MatchingExtensions
{
    public static IQueryable<Vessel> Bookable(this IQueryable<Vessel> vessels) =>
        vessels.Where(v => !v.IsArchived
                        && v.Status == VesselStatus.Available
                        && !v.Offers.Any(o => o.Status == OfferStatus.Pending));

    public static IQueryable<CarrierRoute> Matching(
        this IQueryable<CarrierRoute> routes, Guid originId, Guid destinationId, decimal weight) =>
        routes.Where(r => r.IsActive
                       && r.OriginNileBerthId == originId
                       && r.DestinationNileBerthId == destinationId
                       && r.CarrierProfile.Vessels.AsQueryable().Bookable().Any(v => v.Capacity >= weight));
}
