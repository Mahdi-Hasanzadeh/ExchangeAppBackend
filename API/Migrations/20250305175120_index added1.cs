using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class indexadded1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomerAccounts_Id_AccountType",
                table: "CustomerAccounts");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAccounts_AccountType_Id",
                table: "CustomerAccounts",
                columns: new[] { "AccountType", "Id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomerAccounts_AccountType_Id",
                table: "CustomerAccounts");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAccounts_Id_AccountType",
                table: "CustomerAccounts",
                columns: new[] { "Id", "AccountType" });
        }
    }
}
