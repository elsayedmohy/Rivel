namespace RiverLine.Api.Models.Dtos.Vessel;

public record VesselDto(
    Guid Id,
    string Name,
    string RegistrationNumber,
    VesselType Type,
    decimal Capacity,
    VesselStatus Status,
    int? YearBuilt,
    VesselAssignmentDto? ActiveShipment,
    int PendingOfferCount);  


public record VesselAssignmentDto(
    Guid ShipmentId,
    string CargoType,
    string Origin,
    string Destination,
    ShipmentStatus Status,
    DateOnly PickupDate);