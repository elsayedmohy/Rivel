namespace RiverLine.Api.Models.Dtos.Vessel;

public record CreateVesselDto(
    string Name,
    VesselType Type,
    string RegistrationNumber,
    double Capacity
    );