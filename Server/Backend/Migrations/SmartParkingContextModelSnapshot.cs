﻿// <auto-generated />
using System;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Backend.Migrations
{
    [DbContext(typeof(SmartParkingContext))]
    partial class SmartParkingContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.4");

            modelBuilder.Entity("Backend.Models.Fine", b =>
                {
                    b.Property<string>("Email")
                        .HasColumnType("TEXT")
                        .HasColumnName("email");

                    b.Property<DateTime>("DatetimeStart")
                        .HasColumnType("DATETIME")
                        .HasColumnName("datetime_start");

                    b.Property<DateTime>("DatetimeEnd")
                        .HasColumnType("DATETIME")
                        .HasColumnName("datetime_end");

                    b.Property<bool?>("Paid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("BOOLEAN")
                        .HasColumnName("paid")
                        .HasDefaultValueSql("FALSE");

                    b.HasKey("Email", "DatetimeStart");

                    b.ToTable("fines", null, t =>
                        {
                            t.HasCheckConstraint("CK_Fine_Datetime_Valid", "\"datetime_start\" < \"datetime_end\"");
                        });
                });

            modelBuilder.Entity("Backend.Models.Price", b =>
                {
                    b.Property<string>("Type")
                        .HasColumnType("TEXT")
                        .HasColumnName("type");

                    b.Property<decimal>("Amount")
                        .HasColumnType("DECIMAL(5,2)")
                        .HasColumnName("amount");

                    b.HasKey("Type");

                    b.ToTable("prices", null, t =>
                        {
                            t.HasCheckConstraint("CK_Amount_Positive", "\"amount\" > 0");
                        });

                    b.HasData(
                        new
                        {
                            Type = "PARKING",
                            Amount = 5.5m
                        },
                        new
                        {
                            Type = "CHARGING",
                            Amount = 9.5m
                        });
                });

            modelBuilder.Entity("Backend.Models.PricesType", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.HasKey("Name");

                    b.ToTable("prices_type", (string)null);

                    b.HasData(
                        new
                        {
                            Name = "PARKING"
                        },
                        new
                        {
                            Name = "CHARGING"
                        });
                });

            modelBuilder.Entity("Backend.Models.Request", b =>
                {
                    b.Property<string>("Email")
                        .HasColumnType("TEXT")
                        .HasColumnName("email");

                    b.Property<DateTime>("DatetimeStart")
                        .HasColumnType("DATETIME")
                        .HasColumnName("datetime_start");

                    b.Property<DateTime>("DatetimeEnd")
                        .HasColumnType("DATETIME")
                        .HasColumnName("datetime_end");

                    b.Property<int?>("Kw")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("kw")
                        .HasDefaultValueSql("NULL");

                    b.Property<bool?>("Paid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("BOOLEAN")
                        .HasColumnName("paid")
                        .HasDefaultValueSql("FALSE");

                    b.Property<int>("SlotId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("slot_id");

                    b.HasKey("Email", "DatetimeStart");

                    b.HasIndex("SlotId");

                    b.ToTable("requests", null, t =>
                        {
                            t.HasCheckConstraint("CK_Request_Datetime_Valid", "\"datetime_start\" < \"datetime_end\"");

                            t.HasCheckConstraint("CK_Request_Kw_Valid", "\"kw\" IS NULL OR (\"kw\" > 0 AND \"kw\" <= 100)");
                        });
                });

            modelBuilder.Entity("Backend.Models.Reservation", b =>
                {
                    b.Property<string>("Email")
                        .HasColumnType("TEXT")
                        .HasColumnName("email");

                    b.Property<DateTime>("DatetimeStart")
                        .HasColumnType("DATETIME")
                        .HasColumnName("datetime_start");

                    b.Property<DateTime>("DatetimeEnd")
                        .HasColumnType("DATETIME")
                        .HasColumnName("datetime_end");

                    b.Property<int>("SlotId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("slot_id");

                    b.HasKey("Email", "DatetimeStart");

                    b.HasIndex("SlotId");

                    b.ToTable("reservations", null, t =>
                        {
                            t.HasCheckConstraint("CK_Reservation_Datetime_Valid", "\"datetime_start\" < \"datetime_end\"");
                        });
                });

            modelBuilder.Entity("Backend.Models.Slot", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("status");

                    b.HasKey("Id");

                    b.HasIndex("Status");

                    b.ToTable("slots", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Status = "FREE"
                        },
                        new
                        {
                            Id = 2,
                            Status = "FREE"
                        },
                        new
                        {
                            Id = 3,
                            Status = "FREE"
                        },
                        new
                        {
                            Id = 4,
                            Status = "FREE"
                        },
                        new
                        {
                            Id = 5,
                            Status = "FREE"
                        });
                });

            modelBuilder.Entity("Backend.Models.SlotsStatus", b =>
                {
                    b.Property<string>("Status")
                        .HasColumnType("TEXT")
                        .HasColumnName("status");

                    b.HasKey("Status");

                    b.ToTable("slots_status", (string)null);

                    b.HasData(
                        new
                        {
                            Status = "FREE"
                        },
                        new
                        {
                            Status = "OCCUPIED"
                        });
                });

            modelBuilder.Entity("Backend.Models.User", b =>
                {
                    b.Property<string>("Email")
                        .HasColumnType("TEXT")
                        .HasColumnName("email");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("password");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("surname");

                    b.Property<string>("Type")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValue("BASE")
                        .HasColumnName("type");

                    b.HasKey("Email");

                    b.HasIndex("Type");

                    b.HasIndex(new[] { "Email", "Password" }, "IX_users_email_password")
                        .IsUnique();

                    b.ToTable("users", (string)null);

                    b.HasData(
                        new
                        {
                            Email = "admin@gmail.com",
                            Name = "Matteo",
                            Password = "Admin123",
                            Surname = "Palmieri",
                            Type = "ADMIN"
                        });
                });

            modelBuilder.Entity("Backend.Models.UsersType", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.HasKey("Name");

                    b.ToTable("users_type", (string)null);

                    b.HasData(
                        new
                        {
                            Name = "ADMIN"
                        },
                        new
                        {
                            Name = "BASE"
                        },
                        new
                        {
                            Name = "PREMIUM"
                        });
                });

            modelBuilder.Entity("Backend.Models.Fine", b =>
                {
                    b.HasOne("Backend.Models.User", "EmailNavigation")
                        .WithMany("Fines")
                        .HasForeignKey("Email")
                        .IsRequired();

                    b.Navigation("EmailNavigation");
                });

            modelBuilder.Entity("Backend.Models.Price", b =>
                {
                    b.HasOne("Backend.Models.PricesType", "TypeNavigation")
                        .WithOne("Price")
                        .HasForeignKey("Backend.Models.Price", "Type")
                        .IsRequired();

                    b.Navigation("TypeNavigation");
                });

            modelBuilder.Entity("Backend.Models.Request", b =>
                {
                    b.HasOne("Backend.Models.User", "EmailNavigation")
                        .WithMany("Requests")
                        .HasForeignKey("Email")
                        .IsRequired();

                    b.HasOne("Backend.Models.Slot", "Slot")
                        .WithMany("Requests")
                        .HasForeignKey("SlotId")
                        .IsRequired();

                    b.Navigation("EmailNavigation");

                    b.Navigation("Slot");
                });

            modelBuilder.Entity("Backend.Models.Reservation", b =>
                {
                    b.HasOne("Backend.Models.User", "EmailNavigation")
                        .WithMany("Reservations")
                        .HasForeignKey("Email")
                        .IsRequired();

                    b.HasOne("Backend.Models.Slot", "Slot")
                        .WithMany("Reservations")
                        .HasForeignKey("SlotId")
                        .IsRequired();

                    b.Navigation("EmailNavigation");

                    b.Navigation("Slot");
                });

            modelBuilder.Entity("Backend.Models.Slot", b =>
                {
                    b.HasOne("Backend.Models.SlotsStatus", "StatusNavigation")
                        .WithMany("Slots")
                        .HasForeignKey("Status")
                        .IsRequired();

                    b.Navigation("StatusNavigation");
                });

            modelBuilder.Entity("Backend.Models.User", b =>
                {
                    b.HasOne("Backend.Models.UsersType", "TypeNavigation")
                        .WithMany("Users")
                        .HasForeignKey("Type")
                        .IsRequired();

                    b.Navigation("TypeNavigation");
                });

            modelBuilder.Entity("Backend.Models.PricesType", b =>
                {
                    b.Navigation("Price");
                });

            modelBuilder.Entity("Backend.Models.Slot", b =>
                {
                    b.Navigation("Requests");

                    b.Navigation("Reservations");
                });

            modelBuilder.Entity("Backend.Models.SlotsStatus", b =>
                {
                    b.Navigation("Slots");
                });

            modelBuilder.Entity("Backend.Models.User", b =>
                {
                    b.Navigation("Fines");

                    b.Navigation("Requests");

                    b.Navigation("Reservations");
                });

            modelBuilder.Entity("Backend.Models.UsersType", b =>
                {
                    b.Navigation("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
