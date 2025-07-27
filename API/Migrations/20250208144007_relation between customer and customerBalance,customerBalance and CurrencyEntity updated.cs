using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class relationbetweencustomerandcustomerBalancecustomerBalanceandCurrencyEntityupdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomerBalances_CustomerId",
                table: "CustomerBalances");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "CustomerTransactionHistories");

            migrationBuilder.DropColumn(
                name: "AFN",
                table: "CustomerBalances");

            migrationBuilder.DropColumn(
                name: "EUR",
                table: "CustomerBalances");

            migrationBuilder.DropColumn(
                name: "IRR",
                table: "CustomerBalances");

            migrationBuilder.DropColumn(
                name: "PKR",
                table: "CustomerBalances");

            migrationBuilder.DropColumn(
                name: "TotalBalanceInUSD",
                table: "CustomerBalances");

            migrationBuilder.DropColumn(
                name: "USD",
                table: "CustomerBalances");

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "CustomerTransactionHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "CustomerBalances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerTransactionHistories_CurrencyId",
                table: "CustomerTransactionHistories",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerBalances_CurrencyId",
                table: "CustomerBalances",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerBalances_CustomerId",
                table: "CustomerBalances",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerBalances_Currencies_CurrencyId",
                table: "CustomerBalances",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "CurrencyId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerTransactionHistories_Currencies_CurrencyId",
                table: "CustomerTransactionHistories",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "CurrencyId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerBalances_Currencies_CurrencyId",
                table: "CustomerBalances");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerTransactionHistories_Currencies_CurrencyId",
                table: "CustomerTransactionHistories");

            migrationBuilder.DropIndex(
                name: "IX_CustomerTransactionHistories_CurrencyId",
                table: "CustomerTransactionHistories");

            migrationBuilder.DropIndex(
                name: "IX_CustomerBalances_CurrencyId",
                table: "CustomerBalances");

            migrationBuilder.DropIndex(
                name: "IX_CustomerBalances_CustomerId",
                table: "CustomerBalances");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "CustomerTransactionHistories");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "CustomerBalances");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "CustomerTransactionHistories",
                type: "nvarchar(30)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "AFN",
                table: "CustomerBalances",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "EUR",
                table: "CustomerBalances",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "IRR",
                table: "CustomerBalances",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PKR",
                table: "CustomerBalances",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalBalanceInUSD",
                table: "CustomerBalances",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "USD",
                table: "CustomerBalances",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerBalances_CustomerId",
                table: "CustomerBalances",
                column: "CustomerId",
                unique: true);
        }
    }
}
