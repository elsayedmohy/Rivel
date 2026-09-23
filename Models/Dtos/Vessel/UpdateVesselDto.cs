namespace RiverLine.Api.Models.Dtos.Vessel;

public record UpdateVesselDto(
    string Name,
    string RegistrationNumber,
    VesselType Type,
    decimal Capacity,
    int? YearBuilt);
 
public record SetVesselStatusDto(VesselStatus Status);