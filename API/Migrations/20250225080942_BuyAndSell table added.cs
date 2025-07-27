using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class BuyAndSelltableadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BuyAndSellTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    BuyTransactionId = table.Column<int>(type: "int", nullable: false),
                    SellTransactionId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    SourceCurrencyId = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TargetCurrencyId = table.Column<int>(type: "int", nullable: false),
                    ConvertedAmount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyAndSellTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuyAndSellTransactions_Currencies_SourceCurrencyId",
                        column: x => x.SourceCurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyAndSellTransactions_Currencies_TargetCurrencyId",
                        column: x => x.TargetCurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyAndSellTransactions_CustomerTransactionHistories_BuyTransactionId",
                        column: x => x.BuyTransactionId,
                        principalTable: "CustomerTransactionHistories",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyAndSellTransactions_CustomerTransactionHistories_SellTransactionId",
                        column: x => x.SellTransactionId,
                        principalTable: "CustomerTransactionHistories",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyAndSellTransactions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BuyAndSellTransactions_BuyTransactionId",
                table: "BuyAndSellTransactions",
                column: "BuyTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyAndSellTransactions_SellTransactionId",
                table: "BuyAndSellTransactions",
                column: "SellTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyAndSellTransactions_SourceCurrencyId",
                table: "BuyAndSellTransactions",
                column: "SourceCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyAndSellTransactions_TargetCurrencyId",
                table: "BuyAndSellTransactions",
                column: "TargetCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyAndSellTransactions_UserId",
                table: "BuyAndSellTransactions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BuyAndSellTransactions");
        }
    }
}
