using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public class SmartParkingContext : DbContext
{
    public DbSet<PriceType> PriceTypes { get; set; }
    public DbSet<UserType> UserTypes { get; set; }
    public DbSet<SlotStatus> SlotStatuses { get; set; }

    public DbSet<Price> Prices { get; set; }
    public DbSet<Slot> Slots { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=SmartParking.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PriceType>(e =>
        {
            // Set PK as string
            e.HasKey(p => p.Name);
            e.Property(p => p.Name).HasConversion<string>();

            // Seed enum data
            e.HasData(
                new PriceType { Name = PriceTypeEnum.PARKING },
                new PriceType { Name = PriceTypeEnum.CHARGING }
            );
        });

        modelBuilder.Entity<UserType>(e =>
        {
            // Set PK as string
            e.HasKey(p => p.Name);
            e.Property(p => p.Name).HasConversion<string>();

            // Seed enum data
            e.HasData(
                new UserType { Name = UserTypeEnum.ADMIN },
                new UserType { Name = UserTypeEnum.BASE },
                new UserType { Name = UserTypeEnum.PREMIUM }
            );
        });

        modelBuilder.Entity<SlotStatus>(e =>
        {
            // Set PK as string
            e.HasKey(p => p.Name);
            e.Property(p => p.Name).HasConversion<string>();

            // Seed enum data
            e.HasData(
                new SlotStatus { Name = SlotStatusEnum.FREE },
                new SlotStatus { Name = SlotStatusEnum.OCCUPIED }
            );
        });

        modelBuilder.Entity<Price>(p =>
        {
            // Set PK as string
            p.HasKey(pk => pk.Type);
            p.Property(pk => pk.Type).HasConversion<string>();

            // Set PK as FK to PriceType
            p.HasOne(p => p.PriceType)
                .WithOne(p => p.Price)
                .HasForeignKey<Price>(fk => fk.Type);

            // Set Amount as double with range -999.99 to 999.99
            p.Property(p => p.Amount).HasPrecision(5, 2);

            // Ensure that the Amount is greater than 0.0
            p.ToTable(c => c.HasCheckConstraint("CK_Price_Amount", "Amount > 0.0"));

            // Seed data
            p.HasData(
                new Price { Type = PriceTypeEnum.PARKING, Amount = 5.50 },
                new Price { Type = PriceTypeEnum.CHARGING, Amount = 7.25 }
            );
        });

        modelBuilder.Entity<Slot>(s =>
        {
            // Set PK
            s.HasKey(pk => pk.Id);

            // Set Id as Primary Key with autoincrement
            s.Property(pk => pk.Id)
                .ValueGeneratedOnAdd();

            // Set Status as FK to SlotStatus
            s.HasOne(s => s.SlotStatus)
                .WithMany(s => s.Slots)
                .HasForeignKey(fk => fk.Status);

            // Seed data
            s.HasData(
                new Slot { Id = 1, Status = SlotStatusEnum.FREE },
                new Slot { Id = 2, Status = SlotStatusEnum.FREE },
                new Slot { Id = 3, Status = SlotStatusEnum.FREE },
                new Slot { Id = 4, Status = SlotStatusEnum.FREE },
                new Slot { Id = 5, Status = SlotStatusEnum.FREE }
            );
        });
    }
}
