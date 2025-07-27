using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class indexadded2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomerBalances_UserId_CustomerId_CurrencyId",
                table: "CustomerBalances");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerBalances_UserId_CustomerId_CurrencyId",
                table: "CustomerBalances",
                columns: new[] { "UserId", "CustomerId", "CurrencyId" })
                .Annotation("SqlServer:Include", new[] { "Balance" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomerBalances_UserId_CustomerId_CurrencyId",
                table: "CustomerBalances");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerBalances_UserId_CustomerId_CurrencyId",
                table: "CustomerBalances",
                columns: new[] { "UserId", "CustomerId", "CurrencyId" });
        }
    }
}
