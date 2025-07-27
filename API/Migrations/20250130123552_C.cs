using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class C : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerBalance_CustomerAccounts_CustomerId",
                table: "CustomerBalance");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerBalance",
                table: "CustomerBalance");

            migrationBuilder.RenameTable(
                name: "CustomerBalance",
                newName: "CustomerBalances");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerBalance_CustomerId",
                table: "CustomerBalances",
                newName: "IX_CustomerBalances_CustomerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerBalances",
                table: "CustomerBalances",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerBalances_CustomerAccounts_CustomerId",
                table: "CustomerBalances",
                column: "CustomerId",
                principalTable: "CustomerAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerBalances_CustomerAccounts_CustomerId",
                table: "CustomerBalances");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerBalances",
                table: "CustomerBalances");

            migrationBuilder.RenameTable(
                name: "CustomerBalances",
                newName: "CustomerBalance");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerBalances_CustomerId",
                table: "CustomerBalance",
                newName: "IX_CustomerBalance_CustomerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerBalance",
                table: "CustomerBalance",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerBalance_CustomerAccounts_CustomerId",
                table: "CustomerBalance",
                column: "CustomerId",
                principalTable: "CustomerAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
