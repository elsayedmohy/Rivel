namespace RiverLine.Api.Services.NileBerths;

public class NileBerthService(ApplicationDbContext dbContext) :INileBerthService
{
    public async  Task<Result<List<NileBerthDto>>> GetNileBerthsAsync()
    {

        var berths = await dbContext.NileBerths
            .Where(b => b.IsActive)
            .OrderBy(b => b.Axis)
            .ThenBy(b => b.Latitude)
            .Select(b => new NileBerthDto(
                b.Id,
                b.Name,
                b.ArabicName,
                b.Governorate,
                b.Latitude,
                b.Longitude,
                b.Type.ToString(),
                b.Axis.ToString(),
                b.CoordinateAccuracy.ToString()
            )).ToListAsync();
        
        return Result<List<NileBerthDto>>.Success(berths);

    }
}