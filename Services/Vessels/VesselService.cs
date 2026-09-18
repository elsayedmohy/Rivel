namespace RiverLine.Api.Services.Vessels;


public class VesselService(ApplicationDbContext dbContext) : IVesselService
{
    public async Task<Result<VesselDto>> CreateAsync(Guid carrierId, CreateVesselDto dto)
    {
        var profile = await dbContext.CarrierProfiles
            .FirstOrDefaultAsync(cp => cp.UserId == carrierId);

        if (profile is null)
            return Result<VesselDto>.Failure(OperationError.NotFound,
                "Carrier profile not found. Please complete your profile first.");

        var vessel = new Vessel
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            CarrierProfileId = profile.Id,
            Capacity = dto.Capacity,
            RegistrationNumber = dto.RegistrationNumber,
            Type = dto.Type,
            Status = VesselStatus.Available
        };

        dbContext.Vessels.Add(vessel);
        await dbContext.SaveChangesAsync();

        return Result<VesselDto>.Success(ToDto(vessel));
    }

    public async Task<Result<List<VesselDto>>> GetMineAsync(Guid carrierId)
    {
        var vessels = await dbContext.Vessels
            .Where(v => v.CarrierProfile.UserId == carrierId)
            .Select(x => ToDto(x))
            .ToListAsync();
        // .Select(v =>
            //     new VesselDto(v.Id,
            //         v.Name,
            //         v.Type,
            //         v.RegistrationNumber,
            //         v.Capacity,
            //         v.CapacityUnit,
            //         v.Status.ToString()
            //         ))
            // .ToListAsync();

        return Result<List<VesselDto>>.Success(vessels);
    }

    private static VesselDto ToDto(Vessel v) =>
        new(v.Id,
            v.Name,
            v.Type.ToString(),
            v.RegistrationNumber,
            v.Capacity,
            "Tons",
            v.Status.ToString());
}