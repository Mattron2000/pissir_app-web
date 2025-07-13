using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public partial class SmartParkingContext : DbContext
{
    public SmartParkingContext()
    {
    }

    public SmartParkingContext(DbContextOptions<SmartParkingContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Fine> Fines { get; set; }

    public virtual DbSet<Price> Prices { get; set; }

    public virtual DbSet<PricesType> PricesTypes { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<Slot> Slots { get; set; }

    public virtual DbSet<SlotsStatus> SlotsStatuses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UsersType> UsersTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite("Name=SmartParking");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Fine>(entity =>
        {
            entity.HasKey(e => new { e.Email, e.DatetimeStart });

            entity.ToTable("fines");

            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.DatetimeStart)
                .HasColumnType("DATETIME")
                .HasColumnName("datetime_start");
            entity.Property(e => e.DatetimeEnd)
                .HasColumnType("DATETIME")
                .HasColumnName("datetime_end");
            entity.Property(e => e.Paid)
                .HasDefaultValueSql("FALSE")
                .HasColumnType("BOOLEAN")
                .HasColumnName("paid");

            entity.HasOne(d => d.EmailNavigation).WithMany(p => p.Fines)
                .HasForeignKey(d => d.Email)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Price>(entity =>
        {
            entity.HasKey(e => e.Type);

            entity.ToTable("prices");

            entity.Property(e => e.Type).HasColumnName("type");
            entity.Property(e => e.Amount)
                .HasColumnType("DECIMAL(5,2)")
                .HasColumnName("amount");

            entity.HasOne(d => d.TypeNavigation).WithOne(p => p.Price)
                .HasForeignKey<Price>(d => d.Type)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PricesType>(entity =>
        {
            entity.HasKey(e => e.Name);

            entity.ToTable("prices_type");

            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => new { e.Email, e.DatetimeStart });

            entity.ToTable("requests");

            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.DatetimeStart)
                .HasColumnType("DATETIME")
                .HasColumnName("datetime_start");
            entity.Property(e => e.DatetimeEnd)
                .HasColumnType("DATETIME")
                .HasColumnName("datetime_end");
            entity.Property(e => e.Kw)
                .HasDefaultValueSql("NULL")
                .HasColumnName("kw");
            entity.Property(e => e.Paid)
                .HasDefaultValueSql("FALSE")
                .HasColumnType("BOOLEAN")
                .HasColumnName("paid");
            entity.Property(e => e.SlotId).HasColumnName("slot_id");

            entity.HasOne(d => d.EmailNavigation).WithMany(p => p.Requests)
                .HasForeignKey(d => d.Email)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Slot).WithMany(p => p.Requests)
                .HasForeignKey(d => d.SlotId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => new { e.Email, e.DatetimeStart });

            entity.ToTable("reservations");

            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.DatetimeStart)
                .HasColumnType("DATETIME")
                .HasColumnName("datetime_start");
            entity.Property(e => e.DatetimeEnd)
                .HasColumnType("DATETIME")
                .HasColumnName("datetime_end");
            entity.Property(e => e.SlotId).HasColumnName("slot_id");

            entity.HasOne(d => d.EmailNavigation).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.Email)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Slot).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.SlotId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Slot>(entity =>
        {
            entity.ToTable("slots");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.StatusNavigation).WithMany(p => p.Slots)
                .HasForeignKey(d => d.Status)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<SlotsStatus>(entity =>
        {
            entity.HasKey(e => e.Status);

            entity.ToTable("slots_status");

            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Email);

            entity.ToTable("users");

            entity.HasIndex(e => new { e.Email, e.Password }, "IX_users_email_password").IsUnique();

            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.Surname).HasColumnName("surname");
            entity.Property(e => e.Type)
                .HasDefaultValue("BASE")
                .HasColumnName("type");

            entity.HasOne(d => d.TypeNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.Type)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<UsersType>(entity =>
        {
            entity.HasKey(e => e.Name);

            entity.ToTable("users_type");

            entity.Property(e => e.Name).HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
