using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public partial class SmartParkingContext
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Performance",
        "CA1822:Mark members as static",
        Justification = "Required for extensibility in partial methods and compatibility with EF Core's instance-based model configuration."
    )]
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UsersType>(entity =>
        {
            // Seed enum data
            entity.HasData(
                Enum.GetValues(typeof(UsersTypeEnum))
                    .Cast<UsersTypeEnum>()
                    .Select(e => new UsersType { Name = e.ToString() })
            );
        });

        modelBuilder.Entity<PricesType>(entity =>
        {
            // Seed enum data
            entity.HasData(
                Enum.GetValues(typeof(PricesTypeEnum))
                    .Cast<PricesTypeEnum>()
                    .Select(e => new PricesType { Name = e.ToString() })
            );
        });

        modelBuilder.Entity<Price>(entity =>
        {
            entity.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Amount_Positive", "\"amount\" > 0");
            });

            // Seed prices data
            entity.HasData(
                new Price { Type = PricesTypeEnum.PARKING.ToString(), Amount = 5.5M },
                new Price { Type = PricesTypeEnum.CHARGING.ToString(), Amount = 9.5M }
            );
        });

        modelBuilder.Entity<SlotsStatus>(entity =>
        {
            //Seed enum data
            entity.HasData(
                Enum.GetValues(typeof(SlotsStatusEnum))
                    .Cast<SlotsStatusEnum>()
                    .Select(e => new SlotsStatus { Status = e.ToString() })
            );
        });

        modelBuilder.Entity<Slot>(entity =>
        {
            // Set autoincrement
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            // Seed slot data
            entity.HasData(
                Enumerable.Range(1, 5).Select(id => new Slot
                {
                    Id = id,
                    Status = SlotsStatusEnum.FREE.ToString()
                })
            );
        });

        modelBuilder.Entity<User>(entity =>
        {
            // Seed admin user data
            entity.HasData(
                new User {
                    Email = "admin@gmail.com",
                    Password = "adminadmin",
                    Type = UsersTypeEnum.ADMIN.ToString(),
                    Name = "Matteo",
                    Surname = "Palmieri",
                }
            );
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Request_Kw_Valid", "\"kw\" IS NULL OR (\"kw\" > 0 AND \"kw\" <= 100)");
                t.HasCheckConstraint("CK_Request_Datetime_Valid", "\"datetime_start\" < \"datetime_end\"");
            });
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Reservation_Datetime_Valid", "\"datetime_start\" < \"datetime_end\"");
            });
        });

        modelBuilder.Entity<Fine>(entity =>
        {
            entity.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Fine_Datetime_Valid", "\"datetime_start\" < \"datetime_end\"");
            });
        });
    }
}
