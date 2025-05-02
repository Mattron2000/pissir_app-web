using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedAndCheckInDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "prices_type",
                column: "name",
                values: new object[]
                {
                    "CHARGING",
                    "PARKING"
                });

            migrationBuilder.InsertData(
                table: "slots_status",
                column: "status",
                values: new object[]
                {
                    "FREE",
                    "OCCUPIED"
                });

            migrationBuilder.InsertData(
                table: "users_type",
                column: "name",
                values: new object[]
                {
                    "ADMIN",
                    "BASE",
                    "PREMIUM"
                });

            migrationBuilder.InsertData(
                table: "prices",
                columns: new[] { "type", "amount" },
                values: new object[,]
                {
                    { "CHARGING", 9.5m },
                    { "PARKING", 5.5m }
                });

            migrationBuilder.InsertData(
                table: "slots",
                columns: new[] { "id", "status" },
                values: new object[,]
                {
                    { 1, "FREE" },
                    { 2, "FREE" },
                    { 3, "FREE" },
                    { 4, "FREE" },
                    { 5, "FREE" }
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "email", "name", "password", "surname", "type" },
                values: new object[] { "admin@gmail.com", "Matteo", "adminadmin", "Palmieri", "ADMIN" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_User_Email_Format",
                table: "users",
                sql: "\"email\" LIKE '%_@%_.%'");

            migrationBuilder.AddCheckConstraint(
                name: "CK_User_Name_Length",
                table: "users",
                sql: "LENGTH(\"name\") > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_User_Password_Length",
                table: "users",
                sql: "LENGTH(\"password\") >= 8");

            migrationBuilder.AddCheckConstraint(
                name: "CK_User_Surname_Length",
                table: "users",
                sql: "LENGTH(\"surname\") > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Reservation_Datetime_Valid",
                table: "reservations",
                sql: "\"datetime_start\" < \"datetime_end\"");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Request_Datetime_Valid",
                table: "requests",
                sql: "\"datetime_start\" < \"datetime_end\"");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Request_Kw_Valid",
                table: "requests",
                sql: "\"kw\" IS NULL OR (\"kw\" > 0 AND \"kw\" <= 100)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Amount_Positive",
                table: "prices",
                sql: "\"amount\" > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Fine_Datetime_Valid",
                table: "fines",
                sql: "\"datetime_start\" < \"datetime_end\"");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_User_Email_Format",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CK_User_Name_Length",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CK_User_Password_Length",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CK_User_Surname_Length",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Reservation_Datetime_Valid",
                table: "reservations");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Request_Datetime_Valid",
                table: "requests");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Request_Kw_Valid",
                table: "requests");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Amount_Positive",
                table: "prices");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Fine_Datetime_Valid",
                table: "fines");

            migrationBuilder.DeleteData(
                table: "prices",
                keyColumn: "type",
                keyValue: "CHARGING");

            migrationBuilder.DeleteData(
                table: "prices",
                keyColumn: "type",
                keyValue: "PARKING");

            migrationBuilder.DeleteData(
                table: "slots",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "slots",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "slots",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "slots",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "slots",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "slots_status",
                keyColumn: "status",
                keyValue: "OCCUPIED");

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "email",
                keyValue: "admin@gmail.com");

            migrationBuilder.DeleteData(
                table: "users_type",
                keyColumn: "name",
                keyValue: "BASE");

            migrationBuilder.DeleteData(
                table: "users_type",
                keyColumn: "name",
                keyValue: "PREMIUM");

            migrationBuilder.DeleteData(
                table: "prices_type",
                keyColumn: "name",
                keyValue: "CHARGING");

            migrationBuilder.DeleteData(
                table: "prices_type",
                keyColumn: "name",
                keyValue: "PARKING");

            migrationBuilder.DeleteData(
                table: "slots_status",
                keyColumn: "status",
                keyValue: "FREE");

            migrationBuilder.DeleteData(
                table: "users_type",
                keyColumn: "name",
                keyValue: "ADMIN");
        }
    }
}
