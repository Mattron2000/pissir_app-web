using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class CreateScaffoldDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "prices_type",
                columns: table => new
                {
                    name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prices_type", x => x.name);
                });

            migrationBuilder.CreateTable(
                name: "slots_status",
                columns: table => new
                {
                    status = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_slots_status", x => x.status);
                });

            migrationBuilder.CreateTable(
                name: "users_type",
                columns: table => new
                {
                    name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users_type", x => x.name);
                });

            migrationBuilder.CreateTable(
                name: "prices",
                columns: table => new
                {
                    type = table.Column<string>(type: "TEXT", nullable: false),
                    amount = table.Column<decimal>(type: "DECIMAL(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prices", x => x.type);
                    table.ForeignKey(
                        name: "FK_prices_prices_type_type",
                        column: x => x.type,
                        principalTable: "prices_type",
                        principalColumn: "name");
                });

            migrationBuilder.CreateTable(
                name: "slots",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    status = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_slots", x => x.id);
                    table.ForeignKey(
                        name: "FK_slots_slots_status_status",
                        column: x => x.status,
                        principalTable: "slots_status",
                        principalColumn: "status");
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    email = table.Column<string>(type: "TEXT", nullable: false),
                    password = table.Column<string>(type: "TEXT", nullable: false),
                    type = table.Column<string>(type: "TEXT", nullable: false, defaultValue: "BASE"),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    surname = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.email);
                    table.ForeignKey(
                        name: "FK_users_users_type_type",
                        column: x => x.type,
                        principalTable: "users_type",
                        principalColumn: "name");
                });

            migrationBuilder.CreateTable(
                name: "fines",
                columns: table => new
                {
                    email = table.Column<string>(type: "TEXT", nullable: false),
                    datetime_start = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    datetime_end = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    paid = table.Column<bool>(type: "BOOLEAN", nullable: true, defaultValueSql: "FALSE")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fines", x => new { x.email, x.datetime_start });
                    table.ForeignKey(
                        name: "FK_fines_users_email",
                        column: x => x.email,
                        principalTable: "users",
                        principalColumn: "email");
                });

            migrationBuilder.CreateTable(
                name: "requests",
                columns: table => new
                {
                    email = table.Column<string>(type: "TEXT", nullable: false),
                    datetime_start = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    datetime_end = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    kw = table.Column<int>(type: "INTEGER", nullable: true, defaultValueSql: "NULL"),
                    paid = table.Column<bool>(type: "BOOLEAN", nullable: true, defaultValueSql: "FALSE"),
                    slot_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_requests", x => new { x.email, x.datetime_start });
                    table.ForeignKey(
                        name: "FK_requests_slots_slot_id",
                        column: x => x.slot_id,
                        principalTable: "slots",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_requests_users_email",
                        column: x => x.email,
                        principalTable: "users",
                        principalColumn: "email");
                });

            migrationBuilder.CreateTable(
                name: "reservations",
                columns: table => new
                {
                    email = table.Column<string>(type: "TEXT", nullable: false),
                    datetime_start = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    datetime_end = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    slot_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reservations", x => new { x.email, x.datetime_start });
                    table.ForeignKey(
                        name: "FK_reservations_slots_slot_id",
                        column: x => x.slot_id,
                        principalTable: "slots",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_reservations_users_email",
                        column: x => x.email,
                        principalTable: "users",
                        principalColumn: "email");
                });

            migrationBuilder.CreateIndex(
                name: "IX_requests_slot_id",
                table: "requests",
                column: "slot_id");

            migrationBuilder.CreateIndex(
                name: "IX_reservations_slot_id",
                table: "reservations",
                column: "slot_id");

            migrationBuilder.CreateIndex(
                name: "IX_slots_status",
                table: "slots",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_users_email_password",
                table: "users",
                columns: new[] { "email", "password" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_type",
                table: "users",
                column: "type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "fines");

            migrationBuilder.DropTable(
                name: "prices");

            migrationBuilder.DropTable(
                name: "requests");

            migrationBuilder.DropTable(
                name: "reservations");

            migrationBuilder.DropTable(
                name: "prices_type");

            migrationBuilder.DropTable(
                name: "slots");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "slots_status");

            migrationBuilder.DropTable(
                name: "users_type");
        }
    }
}
