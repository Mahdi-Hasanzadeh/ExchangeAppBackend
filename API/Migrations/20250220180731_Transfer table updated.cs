using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class Transfertableupdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommisionCurrencyId",
                table: "TransferBetweenAccountHistories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CommisionType",
                table: "TransferBetweenAccountHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "TransferBetweenAccountHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TransferBetweenAccountHistories_CommisionCurrencyId",
                table: "TransferBetweenAccountHistories",
                column: "CommisionCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferBetweenAccountHistories_UserId",
                table: "TransferBetweenAccountHistories",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TransferBetweenAccountHistories_Currencies_CommisionCurrencyId",
                table: "TransferBetweenAccountHistories",
                column: "CommisionCurrencyId",
                principalTable: "Currencies",
                principalColumn: "CurrencyId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransferBetweenAccountHistories_Users_UserId",
                table: "TransferBetweenAccountHistories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransferBetweenAccountHistories_Currencies_CommisionCurrencyId",
                table: "TransferBetweenAccountHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_TransferBetweenAccountHistories_Users_UserId",
                table: "TransferBetweenAccountHistories");

            migrationBuilder.DropIndex(
                name: "IX_TransferBetweenAccountHistories_CommisionCurrencyId",
                table: "TransferBetweenAccountHistories");

            migrationBuilder.DropIndex(
                name: "IX_TransferBetweenAccountHistories_UserId",
                table: "TransferBetweenAccountHistories");

            migrationBuilder.DropColumn(
                name: "CommisionCurrencyId",
                table: "TransferBetweenAccountHistories");

            migrationBuilder.DropColumn(
                name: "CommisionType",
                table: "TransferBetweenAccountHistories");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TransferBetweenAccountHistories");
        }
    }
}
