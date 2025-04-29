using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class CreateRequestTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    UserEmail = table.Column<string>(type: "TEXT", nullable: false),
                    DateTimeStart = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    SlotId = table.Column<int>(type: "INTEGER", nullable: false),
                    DateTimeEnd = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    Kw = table.Column<int>(type: "INTEGER", nullable: true),
                    Paid = table.Column<bool>(type: "BOOLEAN", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => new { x.UserEmail, x.DateTimeStart });
                    table.CheckConstraint("CK_Reservation_DateTime", "DateTimeStart < DateTimeEnd");
                    table.ForeignKey(
                        name: "FK_Requests_Slots_SlotId",
                        column: x => x.SlotId,
                        principalTable: "Slots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Requests_Users_UserEmail",
                        column: x => x.UserEmail,
                        principalTable: "Users",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Requests_SlotId",
                table: "Requests",
                column: "SlotId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Requests");
        }
    }
}
