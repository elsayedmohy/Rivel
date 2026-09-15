namespace RiverLine.Api.Extensions;

public static class DevelopmentDataSeeder
{
    public static async Task SeedDevelopmentDataAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Database.MigrateAsync();

        var carriersWithoutVessels = await dbContext.CarrierProfiles
            .Where(cp => !cp.Vessels.Any())
            .ToListAsync();

        foreach (var profile in carriersWithoutVessels)
        {
            dbContext.Vessels.Add(new Vessel
            {
                Id = Guid.NewGuid(),
                CarrierProfileId = profile.Id,
                Capacity = 5000,
                Type = "Barge",
                Status = VesselStatus.Available
            });
        }

        if (carriersWithoutVessels.Count > 0)
            await dbContext.SaveChangesAsync();
    }
}