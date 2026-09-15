namespace RiverLine.Api.Models.Dtos.Offer;

public record CreateOfferDto(decimal Price, DateOnly ProposedPickupDate, Guid VesselId);