using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class currencyExchangeRatetableupdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyExchangeRates_Currencies_CurrencyId",
                table: "CurrencyExchangeRates");

            migrationBuilder.RenameColumn(
                name: "CurrencyId",
                table: "CurrencyExchangeRates",
                newName: "TargetCurrencyId");

            migrationBuilder.RenameIndex(
                name: "IX_CurrencyExchangeRates_CurrencyId",
                table: "CurrencyExchangeRates",
                newName: "IX_CurrencyExchangeRates_TargetCurrencyId");

            migrationBuilder.AddColumn<int>(
                name: "BaseCurrencyId",
                table: "CurrencyExchangeRates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchangeRates_BaseCurrencyId",
                table: "CurrencyExchangeRates",
                column: "BaseCurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyExchangeRates_Currencies_TargetCurrencyId",
                table: "CurrencyExchangeRates",
                column: "TargetCurrencyId",
                principalTable: "Currencies",
                principalColumn: "CurrencyId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyExchangeRates_Currencies_TargetCurrencyId",
                table: "CurrencyExchangeRates");

            migrationBuilder.DropIndex(
                name: "IX_CurrencyExchangeRates_BaseCurrencyId",
                table: "CurrencyExchangeRates");

            migrationBuilder.DropColumn(
                name: "BaseCurrencyId",
                table: "CurrencyExchangeRates");

            migrationBuilder.RenameColumn(
                name: "TargetCurrencyId",
                table: "CurrencyExchangeRates",
                newName: "CurrencyId");

            migrationBuilder.RenameIndex(
                name: "IX_CurrencyExchangeRates_TargetCurrencyId",
                table: "CurrencyExchangeRates",
                newName: "IX_CurrencyExchangeRates_CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyExchangeRates_Currencies_CurrencyId",
                table: "CurrencyExchangeRates",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "CurrencyId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
