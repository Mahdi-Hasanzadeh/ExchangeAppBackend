using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class CustomerBalancetableupdated2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "CustomerBalances",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerBalances_UserId",
                table: "CustomerBalances",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerBalances_Users_UserId",
                table: "CustomerBalances",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerBalances_Users_UserId",
                table: "CustomerBalances");

            migrationBuilder.DropIndex(
                name: "IX_CustomerBalances_UserId",
                table: "CustomerBalances");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CustomerBalances");
        }
    }
}
