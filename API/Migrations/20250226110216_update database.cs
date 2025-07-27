using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class updatedatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "CustomerTransactionHistories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "CurrencyExchangeAccountId",
                table: "BuyAndSellTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BuyAndSellTransactions_CurrencyExchangeAccountId",
                table: "BuyAndSellTransactions",
                column: "CurrencyExchangeAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_BuyAndSellTransactions_CustomerAccounts_CurrencyExchangeAccountId",
                table: "BuyAndSellTransactions",
                column: "CurrencyExchangeAccountId",
                principalTable: "CustomerAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BuyAndSellTransactions_CustomerAccounts_CurrencyExchangeAccountId",
                table: "BuyAndSellTransactions");

            migrationBuilder.DropIndex(
                name: "IX_BuyAndSellTransactions_CurrencyExchangeAccountId",
                table: "BuyAndSellTransactions");

            migrationBuilder.DropColumn(
                name: "CurrencyExchangeAccountId",
                table: "BuyAndSellTransactions");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "CustomerTransactionHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
