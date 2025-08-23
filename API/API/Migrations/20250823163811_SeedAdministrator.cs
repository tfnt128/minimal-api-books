using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdministrator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "administrators",
                columns: new[] { "Id", "Email", "Password", "Profile" },
                values: new object[] { 1, "adm@test.com", "123456", "Adm" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "administrators",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
