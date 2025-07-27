using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class UserPreferencetableadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyExchangeRates_Currencies_TargetCurrencyId",
                table: "CurrencyExchangeRates");

            migrationBuilder.DropTable(
                name: "AccountNumberSequence");

            migrationBuilder.CreateTable(
                name: "UserPreferences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    SettingId = table.Column<int>(type: "int", nullable: false),
                    LastUsedAccountNumber = table.Column<int>(type: "int", nullable: false),
                    CurrencyEntityCurrencyId = table.Column<int>(type: "int", nullable: false),
                    BaseCurrencyId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPreferences_Currencies_CurrencyEntityCurrencyId",
                        column: x => x.CurrencyEntityCurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPreferences_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferences_CurrencyEntityCurrencyId",
                table: "UserPreferences",
                column: "CurrencyEntityCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferences_UserId",
                table: "UserPreferences",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyExchangeRates_Currencies_BaseCurrencyId",
                table: "CurrencyExchangeRates",
                column: "BaseCurrencyId",
                principalTable: "Currencies",
                principalColumn: "CurrencyId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyExchangeRates_Currencies_TargetCurrencyId",
                table: "CurrencyExchangeRates",
                column: "TargetCurrencyId",
                principalTable: "Currencies",
                principalColumn: "CurrencyId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyExchangeRates_Currencies_BaseCurrencyId",
                table: "CurrencyExchangeRates");

            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyExchangeRates_Currencies_TargetCurrencyId",
                table: "CurrencyExchangeRates");

            migrationBuilder.DropTable(
                name: "UserPreferences");

            migrationBuilder.CreateTable(
                name: "AccountNumberSequence",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastUsedAccountNumber = table.Column<int>(type: "int", nullable: false),
                    SettingId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountNumberSequence", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyExchangeRates_Currencies_TargetCurrencyId",
                table: "CurrencyExchangeRates",
                column: "TargetCurrencyId",
                principalTable: "Currencies",
                principalColumn: "CurrencyId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
