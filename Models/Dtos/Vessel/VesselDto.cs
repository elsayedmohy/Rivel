namespace RiverLine.Api.Models.Dtos.Vessel;

public record VesselDto(
    Guid Id,
    string Name,
    string Type,
    string RegistrationNumber,
    double Capacity,
    string CapacityUnit,
    string Status
    );