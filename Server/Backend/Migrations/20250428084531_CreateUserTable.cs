using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class CreateUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Surname = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Email);
                    table.CheckConstraint("CK_User_Name", "Name <> ''");
                    table.CheckConstraint("CK_User_Password", "Length(Password) >= 8");
                    table.CheckConstraint("CK_User_Surname", "Surname <> ''");
                    table.ForeignKey(
                        name: "FK_Users_UserTypes_Type",
                        column: x => x.Type,
                        principalTable: "UserTypes",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Email", "Name", "Password", "Surname", "Type" },
                values: new object[] { "admin@gmail.com", "Matteo", "adminadmin", "Palmieri", "ADMIN" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email_Password",
                table: "Users",
                columns: new[] { "Email", "Password" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Type",
                table: "Users",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
