using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUsersChecks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
