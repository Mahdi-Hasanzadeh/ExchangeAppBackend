using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class indexadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomerBalances_UserId",
                table: "CustomerBalances");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerBalances_UserId_CustomerId_CurrencyId",
                table: "CustomerBalances",
                columns: new[] { "UserId", "CustomerId", "CurrencyId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAccounts_Id_AccountType",
                table: "CustomerAccounts",
                columns: new[] { "Id", "AccountType" });

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_CurrencyId",
                table: "Currencies",
                column: "CurrencyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomerBalances_UserId_CustomerId_CurrencyId",
                table: "CustomerBalances");

            migrationBuilder.DropIndex(
                name: "IX_CustomerAccounts_Id_AccountType",
                table: "CustomerAccounts");

            migrationBuilder.DropIndex(
                name: "IX_Currencies_CurrencyId",
                table: "Currencies");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerBalances_UserId",
                table: "CustomerBalances",
                column: "UserId");
        }
    }
}
