namespace RiverLine.Api.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<CarrierProfile> CarrierProfiles => Set<CarrierProfile>();
    public DbSet<CarrierRoute> CarrierRoutes => Set<CarrierRoute>();
    public DbSet<Vessel> Vessels => Set<Vessel>();
    public DbSet<ShipmentRequest> ShipmentRequests => Set<ShipmentRequest>();
    public DbSet<Offer> Offers => Set<Offer>();
    public DbSet<Shipment> Shipments => Set<Shipment>();
    public DbSet<Rating> Ratings => Set<Rating>();
    public DbSet<NileBerth> NileBerths => Set<NileBerth>();
    
    public DbSet<Notification> Notifications => Set<Notification>();
    


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        NileBerthSeed.Seed(modelBuilder);

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

        modelBuilder.Entity<CarrierProfile>()
            .Property(p => p.Bio)
            .HasMaxLength(1000);

        modelBuilder.Entity<Rating>()
            .Property(r => r.CreatedAt)
            .HasDefaultValueSql("now() at time zone 'utc'");

        modelBuilder.Entity<Vessel>()
            .HasIndex(v => v.RegistrationNumber)
            .IsUnique();

        modelBuilder.Entity<Vessel>()
            .HasIndex(v => new { v.CarrierProfileId, v.IsArchived });

        modelBuilder.Entity<Notification>()
            .HasIndex(n => new { n.UserId, n.IsRead, n.CreatedAt })
            .HasDatabaseName("IX_Notifications_User_Unread");
        
        modelBuilder.Entity<Notification>()
            .Property(n => n.DataJson)
            .HasColumnType("jsonb");
        
        modelBuilder.Entity<Notification>()
            .Property(n => n.CreatedAt)
            .HasDefaultValueSql("now() at time zone 'utc'");
    }
}