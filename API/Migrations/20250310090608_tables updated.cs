using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class tablesupdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BuyAndSellTransactions_Users_UserId",
                table: "BuyAndSellTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Currencies_Users_UserId",
                table: "Currencies");

            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyExchangeRates_Users_UserId",
                table: "CurrencyExchangeRates");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerAccounts_Users_UserId",
                table: "CustomerAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerBalances_Users_UserId",
                table: "CustomerBalances");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerTransactionHistories_Users_UserId",
                table: "CustomerTransactionHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_TransferBetweenAccountHistories_Users_UserId",
                table: "TransferBetweenAccountHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPreferences_Users_UserId",
                table: "UserPreferences");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_UserPreferences_UserId",
                table: "UserPreferences");

            migrationBuilder.DropIndex(
                name: "IX_TransferBetweenAccountHistories_UserId",
                table: "TransferBetweenAccountHistories");

            migrationBuilder.DropIndex(
                name: "IX_CustomerTransactionHistories_UserId",
                table: "CustomerTransactionHistories");

            migrationBuilder.DropIndex(
                name: "IX_BuyAndSellTransactions_UserId",
                table: "BuyAndSellTransactions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentUserId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PictureUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Users_ParentUserId",
                        column: x => x.ParentUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferences_UserId",
                table: "UserPreferences",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransferBetweenAccountHistories_UserId",
                table: "TransferBetweenAccountHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerTransactionHistories_UserId",
                table: "CustomerTransactionHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyAndSellTransactions_UserId",
                table: "BuyAndSellTransactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ParentUserId",
                table: "Users",
                column: "ParentUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BuyAndSellTransactions_Users_UserId",
                table: "BuyAndSellTransactions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Currencies_Users_UserId",
                table: "Currencies",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyExchangeRates_Users_UserId",
                table: "CurrencyExchangeRates",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerAccounts_Users_UserId",
                table: "CustomerAccounts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerBalances_Users_UserId",
                table: "CustomerBalances",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerTransactionHistories_Users_UserId",
                table: "CustomerTransactionHistories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransferBetweenAccountHistories_Users_UserId",
                table: "TransferBetweenAccountHistories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPreferences_Users_UserId",
                table: "UserPreferences",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
