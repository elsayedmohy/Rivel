namespace RiverLine.Api.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<CarrierProfile> CarrierProfiles => Set<CarrierProfile>();
    public DbSet<Vessel> Vessels => Set<Vessel>();
    public DbSet<ShipmentRequest> ShipmentRequests => Set<ShipmentRequest>();
    public DbSet<Offer> Offers => Set<Offer>();
    public DbSet<Shipment> Shipments => Set<Shipment>();
    public DbSet<Rating> Ratings => Set<Rating>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Offer>()
            .HasOne(o => o.Shipment)
            .WithOne(s => s.Offer)
            .HasForeignKey<Shipment>(s => s.OfferId);

        modelBuilder.Entity<Shipment>()
            .HasOne(s => s.Rating)
            .WithOne(r => r.Shipment)
            .HasForeignKey<Rating>(r => r.ShipmentId);

        modelBuilder.Entity<ShipmentRequest>()
            .HasOne(sr => sr.Shipment)
            .WithOne(s => s.ShipmentRequest)
            .HasForeignKey<Shipment>(s => s.ShipmentRequestId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}