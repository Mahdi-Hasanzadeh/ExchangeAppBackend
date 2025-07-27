using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class indexaddedincustomerAccounttable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_TransferBetweenAccountHistories_SenderId",
                table: "TransferBetweenAccountHistories",
                newName: "IX_Transfers_SenderId");

            migrationBuilder.RenameIndex(
                name: "IX_TransferBetweenAccountHistories_RecieverId",
                table: "TransferBetweenAccountHistories",
                newName: "IX_Transfers_RecieverId");

            migrationBuilder.RenameIndex(
                name: "IX_TransferBetweenAccountHistories_CurrencyId",
                table: "TransferBetweenAccountHistories",
                newName: "IX_Transfers_CurrencyId");

            migrationBuilder.AlterColumn<int>(
                name: "AccountType",
                table: "CustomerAccounts",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAccounts_UserId_AccountType",
                table: "CustomerAccounts",
                columns: new[] { "UserId", "AccountType" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomerAccounts_UserId_AccountType",
                table: "CustomerAccounts");

            migrationBuilder.RenameIndex(
                name: "IX_Transfers_SenderId",
                table: "TransferBetweenAccountHistories",
                newName: "IX_TransferBetweenAccountHistories_SenderId");

            migrationBuilder.RenameIndex(
                name: "IX_Transfers_RecieverId",
                table: "TransferBetweenAccountHistories",
                newName: "IX_TransferBetweenAccountHistories_RecieverId");

            migrationBuilder.RenameIndex(
                name: "IX_Transfers_CurrencyId",
                table: "TransferBetweenAccountHistories",
                newName: "IX_TransferBetweenAccountHistories_CurrencyId");

            migrationBuilder.AlterColumn<string>(
                name: "AccountType",
                table: "CustomerAccounts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
