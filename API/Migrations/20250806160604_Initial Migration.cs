using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    CurrencyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Image = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ImageURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Unit = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.CurrencyId);
                });

            migrationBuilder.CreateTable(
                name: "CustomerAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Lastname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    AccountType = table.Column<int>(type: "int", nullable: true),
                    AccountNumber = table.Column<int>(type: "int", nullable: false),
                    BorrowAmount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PassportNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PassportImage = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    RegistrationNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegistrationImage = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Image = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    IDCardNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdCardImage = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OwnerInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    OwnerName = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: false),
                    Logo = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    LogoContentType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnerInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PictureUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ParentUserId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValidUntil = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConnectionString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServerAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Databasename = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isFirstTimeLogin = table.Column<bool>(type: "bit", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "CurrencyExchangeRates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Unit = table.Column<int>(type: "int", nullable: false),
                    Buy = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Sell = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    BaseCurrencyId = table.Column<int>(type: "int", nullable: false),
                    TargetCurrencyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyExchangeRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CurrencyExchangeRates_Currencies_BaseCurrencyId",
                        column: x => x.BaseCurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CurrencyExchangeRates_Currencies_TargetCurrencyId",
                        column: x => x.TargetCurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserPreferences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LastUsedAccountNumber = table.Column<int>(type: "int", nullable: false),
                    BaseCurrencyId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPreferences_Currencies_BaseCurrencyId",
                        column: x => x.BaseCurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerBalances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerBalances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerBalances_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerBalances_CustomerAccounts_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "CustomerAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerTransactionHistories",
                columns: table => new
                {
                    TransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    DepositOrWithdrawBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentNumber = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DealType = table.Column<short>(type: "smallint", nullable: false),
                    TransactionType = table.Column<short>(type: "smallint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerTransactionHistories", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_CustomerTransactionHistories_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerTransactionHistories_CustomerAccounts_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "CustomerAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BuyAndSellTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    SourceCurrencyId = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    ConvertedAmount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CurrencyExchangeAccountId = table.Column<int>(type: "int", nullable: false),
                    SellTransactionId = table.Column<int>(type: "int", nullable: false),
                    CustomerBuyTransactionId = table.Column<int>(type: "int", nullable: true),
                    TargetCurrencyId = table.Column<int>(type: "int", nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    BuyAndSellType = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    BuyTransactionId = table.Column<int>(type: "int", nullable: false),
                    CustomerSellTransactionId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                        name: "FK_BuyAndSellTransactions_CustomerAccounts_CurrencyExchangeAccountId",
                        column: x => x.CurrencyExchangeAccountId,
                        principalTable: "CustomerAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyAndSellTransactions_CustomerAccounts_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "CustomerAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyAndSellTransactions_CustomerTransactionHistories_BuyTransactionId",
                        column: x => x.BuyTransactionId,
                        principalTable: "CustomerTransactionHistories",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyAndSellTransactions_CustomerTransactionHistories_CustomerBuyTransactionId",
                        column: x => x.CustomerBuyTransactionId,
                        principalTable: "CustomerTransactionHistories",
                        principalColumn: "TransactionId");
                    table.ForeignKey(
                        name: "FK_BuyAndSellTransactions_CustomerTransactionHistories_CustomerSellTransactionId",
                        column: x => x.CustomerSellTransactionId,
                        principalTable: "CustomerTransactionHistories",
                        principalColumn: "TransactionId");
                    table.ForeignKey(
                        name: "FK_BuyAndSellTransactions_CustomerTransactionHistories_SellTransactionId",
                        column: x => x.SellTransactionId,
                        principalTable: "CustomerTransactionHistories",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TransferBetweenAccountHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    SenderId = table.Column<int>(type: "int", nullable: false),
                    RecieverId = table.Column<int>(type: "int", nullable: false),
                    CommisionAccountId = table.Column<int>(type: "int", nullable: true),
                    RecieverTransactionId = table.Column<int>(type: "int", nullable: false),
                    SenderTransactionId = table.Column<int>(type: "int", nullable: false),
                    TransactionFeeAccountId = table.Column<int>(type: "int", nullable: true),
                    RecievedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionFeeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SendedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    CommisionCurrencyId = table.Column<int>(type: "int", nullable: true),
                    CommisionType = table.Column<int>(type: "int", nullable: false),
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
                        name: "FK_TransferBetweenAccountHistories_Currencies_CommisionCurrencyId",
                        column: x => x.CommisionCurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferBetweenAccountHistories_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferBetweenAccountHistories_CustomerAccounts_CommisionAccountId",
                        column: x => x.CommisionAccountId,
                        principalTable: "CustomerAccounts",
                        principalColumn: "Id",
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
                name: "IX_BuyAndSellTransactions_BuyTransactionId",
                table: "BuyAndSellTransactions",
                column: "BuyTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyAndSellTransactions_CurrencyExchangeAccountId",
                table: "BuyAndSellTransactions",
                column: "CurrencyExchangeAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyAndSellTransactions_CustomerBuyTransactionId",
                table: "BuyAndSellTransactions",
                column: "CustomerBuyTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyAndSellTransactions_CustomerId",
                table: "BuyAndSellTransactions",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyAndSellTransactions_CustomerSellTransactionId",
                table: "BuyAndSellTransactions",
                column: "CustomerSellTransactionId");

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
                name: "IX_Currencies_CurrencyId",
                table: "Currencies",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_UserId_CurrencyId",
                table: "Currencies",
                columns: new[] { "UserId", "CurrencyId" });

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchangeRates_BaseCurrencyId",
                table: "CurrencyExchangeRates",
                column: "BaseCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchangeRates_EffectiveDate",
                table: "CurrencyExchangeRates",
                column: "EffectiveDate");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchangeRates_IsActive",
                table: "CurrencyExchangeRates",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchangeRates_TargetCurrencyId",
                table: "CurrencyExchangeRates",
                column: "TargetCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchangeRates_UserId",
                table: "CurrencyExchangeRates",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAccounts_UserId",
                table: "CustomerAccounts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAccounts_UserId_AccountNumber",
                table: "CustomerAccounts",
                columns: new[] { "UserId", "AccountNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAccounts_UserId_AccountType",
                table: "CustomerAccounts",
                columns: new[] { "UserId", "AccountType" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerBalances_CurrencyId",
                table: "CustomerBalances",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerBalances_CustomerId",
                table: "CustomerBalances",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerBalances_UserId_CustomerId_CurrencyId",
                table: "CustomerBalances",
                columns: new[] { "UserId", "CustomerId", "CurrencyId" })
                .Annotation("SqlServer:Include", new[] { "Balance" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerTransactionHistories_CurrencyId",
                table: "CustomerTransactionHistories",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerTransactionHistories_CustomerId_TransactionType",
                table: "CustomerTransactionHistories",
                columns: new[] { "CustomerId", "TransactionType" });

            migrationBuilder.CreateIndex(
                name: "IX_OwnerInfos_UserId",
                table: "OwnerInfos",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransferBetweenAccountHistories_CommisionAccountId",
                table: "TransferBetweenAccountHistories",
                column: "CommisionAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferBetweenAccountHistories_CommisionCurrencyId",
                table: "TransferBetweenAccountHistories",
                column: "CommisionCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferBetweenAccountHistories_RecieverTransactionId",
                table: "TransferBetweenAccountHistories",
                column: "RecieverTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferBetweenAccountHistories_SenderTransactionId",
                table: "TransferBetweenAccountHistories",
                column: "SenderTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferBetweenAccountHistories_TransactionFeeAccountId",
                table: "TransferBetweenAccountHistories",
                column: "TransactionFeeAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_CurrencyId",
                table: "TransferBetweenAccountHistories",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_RecieverId",
                table: "TransferBetweenAccountHistories",
                column: "RecieverId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_SenderId",
                table: "TransferBetweenAccountHistories",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferences_BaseCurrencyId",
                table: "UserPreferences",
                column: "BaseCurrencyId",
                unique: true,
                filter: "[BaseCurrencyId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ParentUserId",
                table: "Users",
                column: "ParentUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BuyAndSellTransactions");

            migrationBuilder.DropTable(
                name: "CurrencyExchangeRates");

            migrationBuilder.DropTable(
                name: "CustomerBalances");

            migrationBuilder.DropTable(
                name: "OwnerInfos");

            migrationBuilder.DropTable(
                name: "TransferBetweenAccountHistories");

            migrationBuilder.DropTable(
                name: "UserPreferences");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "CustomerTransactionHistories");

            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.DropTable(
                name: "CustomerAccounts");
        }
    }
}
