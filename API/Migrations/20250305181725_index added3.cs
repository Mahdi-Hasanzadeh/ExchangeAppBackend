using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class indexadded3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Currencies_UserId",
                table: "Currencies");

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_UserId_CurrencyId",
                table: "Currencies",
                columns: new[] { "UserId", "CurrencyId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Currencies_UserId_CurrencyId",
                table: "Currencies");

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_UserId",
                table: "Currencies",
                column: "UserId");
        }
    }
}
