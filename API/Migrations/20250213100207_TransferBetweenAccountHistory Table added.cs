using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class TransferBetweenAccountHistoryTableadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TransferBetweenAccountHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderId = table.Column<int>(type: "int", nullable: false),
                    RecieverId = table.Column<int>(type: "int", nullable: false),
                    RecieverTransactionId = table.Column<int>(type: "int", nullable: false),
                    SenderTransactionId = table.Column<int>(type: "int", nullable: false),
                    TransactionFeeAccountId = table.Column<int>(type: "int", nullable: true),
                    RecievedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionFeeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SendedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SendBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecievedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionFeeRecievedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SenderDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecieverDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionFeeDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferBetweenAccountHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransferBetweenAccountHistories_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferBetweenAccountHistories_CustomerAccounts_RecieverId",
                        column: x => x.RecieverId,
                        principalTable: "CustomerAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferBetweenAccountHistories_CustomerAccounts_SenderId",
                        column: x => x.SenderId,
                        principalTable: "CustomerAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferBetweenAccountHistories_CustomerTransactionHistories_RecieverTransactionId",
                        column: x => x.RecieverTransactionId,
                        principalTable: "CustomerTransactionHistories",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferBetweenAccountHistories_CustomerTransactionHistories_SenderTransactionId",
                        column: x => x.SenderTransactionId,
                        principalTable: "CustomerTransactionHistories",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferBetweenAccountHistories_CustomerTransactionHistories_TransactionFeeAccountId",
                        column: x => x.TransactionFeeAccountId,
                        principalTable: "CustomerTransactionHistories",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransferBetweenAccountHistories_CurrencyId",
                table: "TransferBetweenAccountHistories",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferBetweenAccountHistories_RecieverId",
                table: "TransferBetweenAccountHistories",
                column: "RecieverId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferBetweenAccountHistories_RecieverTransactionId",
                table: "TransferBetweenAccountHistories",
                column: "RecieverTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferBetweenAccountHistories_SenderId",
                table: "TransferBetweenAccountHistories",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferBetweenAccountHistories_SenderTransactionId",
                table: "TransferBetweenAccountHistories",
                column: "SenderTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferBetweenAccountHistories_TransactionFeeAccountId",
                table: "TransferBetweenAccountHistories",
                column: "TransactionFeeAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransferBetweenAccountHistories");
        }
    }
}
