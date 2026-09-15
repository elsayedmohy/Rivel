namespace RiverLine.Api.Models.Dtos.Offer;

public enum OfferOperationError
{
    NotFound,
    Conflict,
    Invalid
}

public sealed record OfferOperationResult(OfferDto? Data, OfferOperationError? Error, string? Message)
{
    public bool Succeeded => Data is not null;

    public static OfferOperationResult Success(OfferDto data) => new(data, null, null);

    public static OfferOperationResult Failure(OfferOperationError error, string? message = null) =>
        new(null, error, message);
}