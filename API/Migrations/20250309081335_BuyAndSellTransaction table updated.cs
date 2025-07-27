using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class BuyAndSellTransactiontableupdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BuyAndSellTransactions_CustomerAccounts_CurrencyExchangeAccountId",
                table: "BuyAndSellTransactions");

            migrationBuilder.AddColumn<int>(
                name: "BuyAndSellType",
                table: "BuyAndSellTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "BuyAndSellTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BuyAndSellTransactions_CustomerId",
                table: "BuyAndSellTransactions",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_BuyAndSellTransactions_CustomerAccounts_CurrencyExchangeAccountId",
                table: "BuyAndSellTransactions",
                column: "CurrencyExchangeAccountId",
                principalTable: "CustomerAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BuyAndSellTransactions_CustomerAccounts_CustomerId",
                table: "BuyAndSellTransactions",
                column: "CustomerId",
                principalTable: "CustomerAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BuyAndSellTransactions_CustomerAccounts_CurrencyExchangeAccountId",
                table: "BuyAndSellTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_BuyAndSellTransactions_CustomerAccounts_CustomerId",
                table: "BuyAndSellTransactions");

            migrationBuilder.DropIndex(
                name: "IX_BuyAndSellTransactions_CustomerId",
                table: "BuyAndSellTransactions");

            migrationBuilder.DropColumn(
                name: "BuyAndSellType",
                table: "BuyAndSellTransactions");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "BuyAndSellTransactions");

            migrationBuilder.AddForeignKey(
                name: "FK_BuyAndSellTransactions_CustomerAccounts_CurrencyExchangeAccountId",
                table: "BuyAndSellTransactions",
                column: "CurrencyExchangeAccountId",
                principalTable: "CustomerAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
