using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

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
                    CurrencyId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Symbol = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Image = table.Column<byte[]>(type: "bytea", nullable: true),
                    ImageURL = table.Column<string>(type: "text", nullable: true),
                    Unit = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.CurrencyId);
                });

            migrationBuilder.CreateTable(
                name: "CustomerAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Lastname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Mobile = table.Column<string>(type: "text", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    AccountType = table.Column<int>(type: "integer", nullable: true),
                    AccountNumber = table.Column<int>(type: "integer", nullable: false),
                    BorrowAmount = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    Address = table.Column<string>(type: "text", nullable: true),
                    PassportNumber = table.Column<string>(type: "text", nullable: true),
                    PassportImage = table.Column<byte[]>(type: "bytea", nullable: true),
                    RegistrationNumber = table.Column<string>(type: "text", nullable: true),
                    RegistrationImage = table.Column<byte[]>(type: "bytea", nullable: true),
                    Image = table.Column<byte[]>(type: "bytea", nullable: true),
                    IDCardNumber = table.Column<string>(type: "text", nullable: true),
                    IdCardImage = table.Column<byte[]>(type: "bytea", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OwnerInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    OwnerName = table.Column<string>(type: "character varying(35)", maxLength: 35, nullable: false),
                    Logo = table.Column<byte[]>(type: "bytea", nullable: true),
                    LogoContentType = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnerInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PictureUrl = table.Column<string>(type: "text", nullable: true),
                    Image = table.Column<byte[]>(type: "bytea", nullable: true),
                    ParentUserId = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ValidUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ConnectionString = table.Column<string>(type: "text", nullable: true),
                    ServerAddress = table.Column<string>(type: "text", nullable: true),
                    Databasename = table.Column<string>(type: "text", nullable: true),
                    isFirstTimeLogin = table.Column<bool>(type: "boolean", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Unit = table.Column<int>(type: "integer", nullable: false),
                    Buy = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Sell = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: true),
                    Remark = table.Column<string>(type: "text", nullable: true),
                    EffectiveDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    BaseCurrencyId = table.Column<int>(type: "integer", nullable: false),
                    TargetCurrencyId = table.Column<int>(type: "integer", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    LastUsedAccountNumber = table.Column<int>(type: "integer", nullable: false),
                    BaseCurrencyId = table.Column<int>(type: "integer", nullable: true)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerId = table.Column<int>(type: "integer", nullable: false),
                    CurrencyId = table.Column<int>(type: "integer", nullable: false),
                    Balance = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
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
                    TransactionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CustomerId = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    CurrencyId = table.Column<int>(type: "integer", nullable: false),
                    DepositOrWithdrawBy = table.Column<string>(type: "text", nullable: false),
                    DocumentNumber = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    DealType = table.Column<short>(type: "smallint", nullable: false),
                    TransactionType = table.Column<short>(type: "smallint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    SourceCurrencyId = table.Column<int>(type: "integer", nullable: false),
                    Rate = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    ConvertedAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CurrencyExchangeAccountId = table.Column<int>(type: "integer", nullable: false),
                    SellTransactionId = table.Column<int>(type: "integer", nullable: false),
                    CustomerBuyTransactionId = table.Column<int>(type: "integer", nullable: true),
                    TargetCurrencyId = table.Column<int>(type: "integer", nullable: false),
                    TransactionType = table.Column<int>(type: "integer", nullable: false),
                    BuyAndSellType = table.Column<int>(type: "integer", nullable: false),
                    CustomerId = table.Column<int>(type: "integer", nullable: true),
                    BuyTransactionId = table.Column<int>(type: "integer", nullable: false),
                    CustomerSellTransactionId = table.Column<int>(type: "integer", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                        name: "FK_BuyAndSellTransactions_CustomerAccounts_CurrencyExchangeAcc~",
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
                        name: "FK_BuyAndSellTransactions_CustomerTransactionHistories_BuyTran~",
                        column: x => x.BuyTransactionId,
                        principalTable: "CustomerTransactionHistories",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BuyAndSellTransactions_CustomerTransactionHistories_Custome~",
                        column: x => x.CustomerBuyTransactionId,
                        principalTable: "CustomerTransactionHistories",
                        principalColumn: "TransactionId");
                    table.ForeignKey(
                        name: "FK_BuyAndSellTransactions_CustomerTransactionHistories_Custom~1",
                        column: x => x.CustomerSellTransactionId,
                        principalTable: "CustomerTransactionHistories",
                        principalColumn: "TransactionId");
                    table.ForeignKey(
                        name: "FK_BuyAndSellTransactions_CustomerTransactionHistories_SellTra~",
                        column: x => x.SellTransactionId,
                        principalTable: "CustomerTransactionHistories",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TransferBetweenAccountHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    SenderId = table.Column<int>(type: "integer", nullable: false),
                    RecieverId = table.Column<int>(type: "integer", nullable: false),
                    CommisionAccountId = table.Column<int>(type: "integer", nullable: true),
                    RecieverTransactionId = table.Column<int>(type: "integer", nullable: false),
                    SenderTransactionId = table.Column<int>(type: "integer", nullable: false),
                    TransactionFeeAccountId = table.Column<int>(type: "integer", nullable: true),
                    RecievedAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TransactionFeeAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    SendedAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CurrencyId = table.Column<int>(type: "integer", nullable: false),
                    CommisionCurrencyId = table.Column<int>(type: "integer", nullable: true),
                    CommisionType = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SendBy = table.Column<string>(type: "text", nullable: false),
                    RecievedBy = table.Column<string>(type: "text", nullable: false),
                    TransactionFeeRecievedBy = table.Column<string>(type: "text", nullable: false),
                    SenderDescription = table.Column<string>(type: "text", nullable: true),
                    RecieverDescription = table.Column<string>(type: "text", nullable: true),
                    TransactionFeeDescription = table.Column<string>(type: "text", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferBetweenAccountHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransferBetweenAccountHistories_Currencies_CommisionCurrenc~",
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
                        name: "FK_TransferBetweenAccountHistories_CustomerAccounts_CommisionA~",
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
                        name: "FK_TransferBetweenAccountHistories_CustomerTransactionHistorie~",
                        column: x => x.RecieverTransactionId,
                        principalTable: "CustomerTransactionHistories",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferBetweenAccountHistories_CustomerTransactionHistori~1",
                        column: x => x.SenderTransactionId,
                        principalTable: "CustomerTransactionHistories",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferBetweenAccountHistories_CustomerTransactionHistori~2",
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
                .Annotation("Npgsql:IndexInclude", new[] { "Balance" });

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
                unique: true);

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
