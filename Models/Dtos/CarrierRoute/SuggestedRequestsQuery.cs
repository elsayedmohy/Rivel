namespace RiverLine.Api.Models.Dtos.CarrierRoute;

public enum SuggestedSort { Newest, PickupSoonest, WeightAsc, WeightDesc }
 
public record SuggestedRequestsQuery(
    Guid? RouteId = null,
    bool FittingOnly = false,
    SuggestedSort Sort = SuggestedSort.Newest,
    int Page = 1,
    int PageSize = 4);