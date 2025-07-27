using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class userIdaddedtotransactionHistorytable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "CustomerTransactionHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerTransactionHistories_UserId",
                table: "CustomerTransactionHistories",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerTransactionHistories_Users_UserId",
                table: "CustomerTransactionHistories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerTransactionHistories_Users_UserId",
                table: "CustomerTransactionHistories");

            migrationBuilder.DropIndex(
                name: "IX_CustomerTransactionHistories_UserId",
                table: "CustomerTransactionHistories");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CustomerTransactionHistories");
        }
    }
}
