using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class CreateEnumTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PriceTypes",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceTypes", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "SlotStatuses",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlotStatuses", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "UserTypes",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTypes", x => x.Name);
                });

            migrationBuilder.InsertData(
                table: "PriceTypes",
                column: "Name",
                values: new object[]
                {
                    "CHARGING",
                    "PARKING"
                });

            migrationBuilder.InsertData(
                table: "SlotStatuses",
                column: "Name",
                values: new object[]
                {
                    "FREE",
                    "OCCUPIED"
                });

            migrationBuilder.InsertData(
                table: "UserTypes",
                column: "Name",
                values: new object[]
                {
                    "ADMIN",
                    "BASE",
                    "PREMIUM"
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PriceTypes");

            migrationBuilder.DropTable(
                name: "SlotStatuses");

            migrationBuilder.DropTable(
                name: "UserTypes");
        }
    }
}
