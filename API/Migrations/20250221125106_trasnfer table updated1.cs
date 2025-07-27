using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class trasnfertableupdated1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommisionAccountId",
                table: "TransferBetweenAccountHistories",
                type: "int",
                nullable: false,
                defaultValue: 81);

            migrationBuilder.CreateIndex(
                name: "IX_TransferBetweenAccountHistories_CommisionAccountId",
                table: "TransferBetweenAccountHistories",
                column: "CommisionAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_TransferBetweenAccountHistories_CustomerAccounts_CommisionAccountId",
                table: "TransferBetweenAccountHistories",
                column: "CommisionAccountId",
                principalTable: "CustomerAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransferBetweenAccountHistories_CustomerAccounts_CommisionAccountId",
                table: "TransferBetweenAccountHistories");

            migrationBuilder.DropIndex(
                name: "IX_TransferBetweenAccountHistories_CommisionAccountId",
                table: "TransferBetweenAccountHistories");

            migrationBuilder.DropColumn(
                name: "CommisionAccountId",
                table: "TransferBetweenAccountHistories");
        }
    }
}
