using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class UserPreferencetableupdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPreferences_Currencies_CurrencyEntityCurrencyId",
                table: "UserPreferences");

            migrationBuilder.DropIndex(
                name: "IX_UserPreferences_CurrencyEntityCurrencyId",
                table: "UserPreferences");

            migrationBuilder.DropColumn(
                name: "CurrencyEntityCurrencyId",
                table: "UserPreferences");

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferences_BaseCurrencyId",
                table: "UserPreferences",
                column: "BaseCurrencyId",
                unique: true,
                filter: "[BaseCurrencyId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPreferences_Currencies_BaseCurrencyId",
                table: "UserPreferences",
                column: "BaseCurrencyId",
                principalTable: "Currencies",
                principalColumn: "CurrencyId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPreferences_Currencies_BaseCurrencyId",
                table: "UserPreferences");

            migrationBuilder.DropIndex(
                name: "IX_UserPreferences_BaseCurrencyId",
                table: "UserPreferences");

            migrationBuilder.AddColumn<int>(
                name: "CurrencyEntityCurrencyId",
                table: "UserPreferences",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferences_CurrencyEntityCurrencyId",
                table: "UserPreferences",
                column: "CurrencyEntityCurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPreferences_Currencies_CurrencyEntityCurrencyId",
                table: "UserPreferences",
                column: "CurrencyEntityCurrencyId",
                principalTable: "Currencies",
                principalColumn: "CurrencyId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
