using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public class SmartParkingContext : DbContext
{
    public DbSet<PriceType> PriceTypes { get; set; }
    public DbSet<UserType> UserTypes { get; set; }
    public DbSet<SlotStatus> SlotStatuses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=SmartParking.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PriceType>(e =>
        {
            // Set PK as string
            e.HasKey(p => p.Name);
            e.Property(p => p.Name).HasConversion<string>();

            // Seed enum data
            e.HasData(
                Enum.GetValues(typeof(PriceTypeEnum))
                    .Cast<PriceTypeEnum>()
                    .Select(e => new PriceType { Name = e })
            );
        });

        modelBuilder.Entity<UserType>(e =>
        {
            // Set PK as string
            e.HasKey(p => p.Name);
            e.Property(p => p.Name).HasConversion<string>();

            // Seed enum data
            e.HasData(
                Enum.GetValues(typeof(UserTypeEnum))
                    .Cast<UserTypeEnum>()
                    .Select(e => new UserType { Name = e })
            );
        });

        modelBuilder.Entity<SlotStatus>(e =>
        {
            // Set PK as string
            e.HasKey(p => p.Name);
            e.Property(p => p.Name).HasConversion<string>();

            // Seed enum data
            e.HasData(
                Enum.GetValues(typeof(SlotStatusEnum))
                    .Cast<SlotStatusEnum>()
                    .Select(e => new SlotStatus { Name = e })
            );
        });
    }
}
