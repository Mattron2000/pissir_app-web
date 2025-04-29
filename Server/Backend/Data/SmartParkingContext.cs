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
    public DbSet<User> Users { get; set; }

    public DbSet<Fine> Fines { get; set; }
    public DbSet<Reservation> Reservations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=SmartParking.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PriceType>(pt =>
        {
            // Set PK as string
            pt.HasKey(p => p.Name);
            pt.Property(p => p.Name).HasConversion<string>();

            // Seed enum data
            pt.HasData(
                new PriceType { Name = PriceTypeEnum.PARKING },
                new PriceType { Name = PriceTypeEnum.CHARGING }
            );
        });

        modelBuilder.Entity<UserType>(ut =>
        {
            // Set PK as string
            ut.HasKey(p => p.Name);
            ut.Property(p => p.Name).HasConversion<string>();

            // Seed enum data
            ut.HasData(
                new UserType { Name = UserTypeEnum.ADMIN },
                new UserType { Name = UserTypeEnum.BASE },
                new UserType { Name = UserTypeEnum.PREMIUM }
            );
        });

        modelBuilder.Entity<SlotStatus>(ss =>
        {
            // Set PK as string
            ss.HasKey(p => p.Name);
            ss.Property(p => p.Name).HasConversion<string>();

            // Seed enum data
            ss.HasData(
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

        modelBuilder.Entity<User>(u =>
        {
            // Set PK
            u.HasKey(pk => pk.Email);

            // Check constraint Password min length
            u.ToTable(c => c.HasCheckConstraint("CK_User_Password", "Length(Password) >= 8"));

            // Set Unique constraint on Email and Password
            u.HasIndex(e => new { e.Email, e.Password })
                .IsUnique();

            // Set FK to UserType
            u.HasOne(e => e.UserType)
                .WithMany(e => e.Users)
                .HasForeignKey(fk => fk.Type);

            // Check Name and Surname that are not empty
            u.ToTable(c => c.HasCheckConstraint("CK_User_Name", "Name <> ''"));
            u.ToTable(c => c.HasCheckConstraint("CK_User_Surname", "Surname <> ''"));

            // Seed Admin data
            u.HasData(
                new User {
                    Email = "admin@gmail.com",
                    Password = "adminadmin",
                    Type = UserTypeEnum.ADMIN,
                    Name = "Matteo",
                    Surname = "Palmieri"
                }
            );
        });

        modelBuilder.Entity<Fine>(f =>
        {
            // Set FK to User
            f.HasOne(f => f.User)
                .WithMany(u => u.Fines)
                .HasForeignKey(fk => fk.UserEmail);

            // Set Datetimes as DATETIME type
            f.Property(f => f.DateTimeStart)
                .HasColumnType("DATETIME");
            f.Property(f => f.DateTimeEnd)
                .HasColumnType("DATETIME");

            // Set PK as composite key (UserEmail + DateTimeStart)
            f.HasKey(pk => new { pk.UserEmail, pk.DateTimeStart });

            // Check Datetimes
            f.ToTable(c => c.HasCheckConstraint("CK_Fine_DateTime", "DateTimeStart < DateTimeEnd"));

            // Set Paid as bool type with default value false
            f.Property(f => f.Paid)
                .HasColumnType("BOOLEAN")
                .HasDefaultValue(false);
        });

        modelBuilder.Entity<Reservation>(r =>
        {
            // Set PK as composite key (UserEmail + DateTimeStart)
            r.HasKey(pk => new { pk.UserEmail, pk.DateTimeStart });

            // Set FK to User
            r.HasOne(r => r.User)
                .WithMany(u => u.Reservations)
                .HasForeignKey(fk => fk.UserEmail);
            // Set FK to Slot
            r.HasOne(r => r.Slot)
                .WithMany(s => s.Reservations)
                .HasForeignKey(fk => fk.SlotId);

            // Set Datetimes as DATETIME type
            r.Property(r => r.DateTimeStart)
                .HasColumnType("DATETIME");
            r.Property(r => r.DateTimeEnd)
                .HasColumnType("DATETIME");

            // Check Datetimes
            r.ToTable(c => c.HasCheckConstraint("CK_Reservation_DateTime", "DateTimeStart < DateTimeEnd"));
        });
    }
}
