using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class columnaddedtobuyAndSellTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerBuyTransactionId",
                table: "BuyAndSellTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomerSellTransactionId",
                table: "BuyAndSellTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BuyAndSellTransactions_CustomerBuyTransactionId",
                table: "BuyAndSellTransactions",
                column: "CustomerBuyTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyAndSellTransactions_CustomerSellTransactionId",
                table: "BuyAndSellTransactions",
                column: "CustomerSellTransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_BuyAndSellTransactions_CustomerTransactionHistories_CustomerBuyTransactionId",
                table: "BuyAndSellTransactions",
                column: "CustomerBuyTransactionId",
                principalTable: "CustomerTransactionHistories",
                principalColumn: "TransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_BuyAndSellTransactions_CustomerTransactionHistories_CustomerSellTransactionId",
                table: "BuyAndSellTransactions",
                column: "CustomerSellTransactionId",
                principalTable: "CustomerTransactionHistories",
                principalColumn: "TransactionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BuyAndSellTransactions_CustomerTransactionHistories_CustomerBuyTransactionId",
                table: "BuyAndSellTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_BuyAndSellTransactions_CustomerTransactionHistories_CustomerSellTransactionId",
                table: "BuyAndSellTransactions");

            migrationBuilder.DropIndex(
                name: "IX_BuyAndSellTransactions_CustomerBuyTransactionId",
                table: "BuyAndSellTransactions");

            migrationBuilder.DropIndex(
                name: "IX_BuyAndSellTransactions_CustomerSellTransactionId",
                table: "BuyAndSellTransactions");

            migrationBuilder.DropColumn(
                name: "CustomerBuyTransactionId",
                table: "BuyAndSellTransactions");

            migrationBuilder.DropColumn(
                name: "CustomerSellTransactionId",
                table: "BuyAndSellTransactions");
        }
    }
}
