using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class indexonCustomerTransactionHistorytable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomerTransactionHistories_CustomerId",
                table: "CustomerTransactionHistories");

            migrationBuilder.AlterColumn<byte>(
                name: "TransactionType",
                table: "CustomerTransactionHistories",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<byte>(
                name: "DealType",
                table: "CustomerTransactionHistories",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerTransactionHistories_CustomerId_TransactionType",
                table: "CustomerTransactionHistories",
                columns: new[] { "CustomerId", "TransactionType" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomerTransactionHistories_CustomerId_TransactionType",
                table: "CustomerTransactionHistories");

            migrationBuilder.AlterColumn<string>(
                name: "TransactionType",
                table: "CustomerTransactionHistories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<string>(
                name: "DealType",
                table: "CustomerTransactionHistories",
                type: "nvarchar(30)",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerTransactionHistories_CustomerId",
                table: "CustomerTransactionHistories",
                column: "CustomerId");
        }
    }
}
