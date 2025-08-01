﻿// <auto-generated />
using System;
using API.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace API.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Shared.Models.BuyAndSellTransaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,4)");

                    b.Property<int>("BuyAndSellType")
                        .HasColumnType("integer");

                    b.Property<int>("BuyTransactionId")
                        .HasColumnType("integer");

                    b.Property<decimal>("ConvertedAmount")
                        .HasColumnType("decimal(18,4)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("CurrencyExchangeAccountId")
                        .HasColumnType("integer");

                    b.Property<int?>("CustomerBuyTransactionId")
                        .HasColumnType("integer");

                    b.Property<int?>("CustomerId")
                        .HasColumnType("integer");

                    b.Property<int?>("CustomerSellTransactionId")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<decimal>("Rate")
                        .HasColumnType("decimal(18,4)");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("bytea");

                    b.Property<int>("SellTransactionId")
                        .HasColumnType("integer");

                    b.Property<int>("SourceCurrencyId")
                        .HasColumnType("integer");

                    b.Property<int>("TargetCurrencyId")
                        .HasColumnType("integer");

                    b.Property<int>("TransactionType")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("BuyTransactionId");

                    b.HasIndex("CurrencyExchangeAccountId");

                    b.HasIndex("CustomerBuyTransactionId");

                    b.HasIndex("CustomerId");

                    b.HasIndex("CustomerSellTransactionId");

                    b.HasIndex("SellTransactionId");

                    b.HasIndex("SourceCurrencyId");

                    b.HasIndex("TargetCurrencyId");

                    b.ToTable("BuyAndSellTransactions");
                });

            modelBuilder.Entity("Shared.Models.Currency.CurrencyEntity", b =>
                {
                    b.Property<int>("CurrencyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("CurrencyId"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)");

                    b.Property<byte[]>("Image")
                        .HasColumnType("bytea");

                    b.Property<string>("ImageURL")
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("Symbol")
                        .HasMaxLength(5)
                        .HasColumnType("character varying(5)");

                    b.Property<int>("Unit")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("CurrencyId");

                    b.HasIndex("CurrencyId");

                    b.HasIndex("UserId", "CurrencyId");

                    b.ToTable("Currencies");
                });

            modelBuilder.Entity("Shared.Models.Currency.CurrencyExchangeRate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("BaseCurrencyId")
                        .HasColumnType("integer");

                    b.Property<decimal>("Buy")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("EffectiveDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("Remark")
                        .HasColumnType("text");

                    b.Property<decimal>("Sell")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Source")
                        .HasColumnType("text");

                    b.Property<int>("TargetCurrencyId")
                        .HasColumnType("integer");

                    b.Property<int>("Unit")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("BaseCurrencyId");

                    b.HasIndex("EffectiveDate");

                    b.HasIndex("IsActive");

                    b.HasIndex("TargetCurrencyId");

                    b.HasIndex("UserId");

                    b.ToTable("CurrencyExchangeRates");
                });

            modelBuilder.Entity("Shared.Models.CustomerAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AccountNumber")
                        .HasColumnType("integer");

                    b.Property<int?>("AccountType")
                        .HasColumnType("integer");

                    b.Property<string>("Address")
                        .HasColumnType("text");

                    b.Property<decimal>("BorrowAmount")
                        .HasPrecision(18, 4)
                        .HasColumnType("numeric(18,4)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("IDCardNumber")
                        .HasColumnType("text");

                    b.Property<byte[]>("IdCardImage")
                        .HasColumnType("bytea");

                    b.Property<byte[]>("Image")
                        .HasColumnType("bytea");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("LastModifiedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Lastname")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("Mobile")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<byte[]>("PassportImage")
                        .HasColumnType("bytea");

                    b.Property<string>("PassportNumber")
                        .HasColumnType("text");

                    b.Property<byte[]>("RegistrationImage")
                        .HasColumnType("bytea");

                    b.Property<string>("RegistrationNumber")
                        .HasColumnType("text");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("bytea");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("UserId", "AccountNumber")
                        .IsUnique();

                    b.HasIndex("UserId", "AccountType");

                    b.ToTable("CustomerAccounts");
                });

            modelBuilder.Entity("Shared.Models.CustomerBalance", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Balance")
                        .HasColumnType("decimal(18,4)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("CurrencyId")
                        .HasColumnType("integer");

                    b.Property<int>("CustomerId")
                        .HasColumnType("integer");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("bytea");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CurrencyId");

                    b.HasIndex("CustomerId");

                    b.HasIndex("UserId", "CustomerId", "CurrencyId");

                    NpgsqlIndexBuilderExtensions.IncludeProperties(b.HasIndex("UserId", "CustomerId", "CurrencyId"), new[] { "Balance" });

                    b.ToTable("CustomerBalances");
                });

            modelBuilder.Entity("Shared.Models.CustomerTransactionHistory", b =>
                {
                    b.Property<int>("TransactionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("TransactionId"));

                    b.Property<decimal>("Amount")
                        .HasPrecision(18, 4)
                        .HasColumnType("numeric(18,4)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("CurrencyId")
                        .HasColumnType("integer");

                    b.Property<int>("CustomerId")
                        .HasColumnType("integer");

                    b.Property<short>("DealType")
                        .HasColumnType("smallint");

                    b.Property<string>("DepositOrWithdrawBy")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<int>("DocumentNumber")
                        .HasColumnType("integer");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("bytea");

                    b.Property<short>("TransactionType")
                        .HasColumnType("smallint");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("TransactionId");

                    b.HasIndex("CurrencyId");

                    b.HasIndex("CustomerId", "TransactionType");

                    b.ToTable("CustomerTransactionHistories");
                });

            modelBuilder.Entity("Shared.Models.Helpers.UserPreference", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("BaseCurrencyId")
                        .HasColumnType("integer");

                    b.Property<int>("LastUsedAccountNumber")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("BaseCurrencyId")
                        .IsUnique();

                    b.ToTable("UserPreferences");
                });

            modelBuilder.Entity("Shared.Models.OwnerInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<byte[]>("Logo")
                        .HasColumnType("bytea");

                    b.Property<string>("LogoContentType")
                        .HasColumnType("text");

                    b.Property<string>("OwnerName")
                        .IsRequired()
                        .HasMaxLength(35)
                        .HasColumnType("character varying(35)");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("OwnerInfos");
                });

            modelBuilder.Entity("Shared.Models.TransferBetweenAccountHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("CommisionAccountId")
                        .HasColumnType("integer");

                    b.Property<int?>("CommisionCurrencyId")
                        .HasColumnType("integer");

                    b.Property<int>("CommisionType")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("CurrencyId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("LastUpdatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<decimal>("RecievedAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("RecievedBy")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RecieverDescription")
                        .HasColumnType("text");

                    b.Property<int>("RecieverId")
                        .HasColumnType("integer");

                    b.Property<int>("RecieverTransactionId")
                        .HasColumnType("integer");

                    b.Property<byte[]>("RowVersion")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<string>("SendBy")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("SendedAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("SenderDescription")
                        .HasColumnType("text");

                    b.Property<int>("SenderId")
                        .HasColumnType("integer");

                    b.Property<int>("SenderTransactionId")
                        .HasColumnType("integer");

                    b.Property<int?>("TransactionFeeAccountId")
                        .HasColumnType("integer");

                    b.Property<decimal>("TransactionFeeAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("TransactionFeeDescription")
                        .HasColumnType("text");

                    b.Property<string>("TransactionFeeRecievedBy")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CommisionAccountId");

                    b.HasIndex("CommisionCurrencyId");

                    b.HasIndex("CurrencyId")
                        .HasDatabaseName("IX_Transfers_CurrencyId");

                    b.HasIndex("RecieverId")
                        .HasDatabaseName("IX_Transfers_RecieverId");

                    b.HasIndex("RecieverTransactionId");

                    b.HasIndex("SenderId")
                        .HasDatabaseName("IX_Transfers_SenderId");

                    b.HasIndex("SenderTransactionId");

                    b.HasIndex("TransactionFeeAccountId");

                    b.ToTable("TransferBetweenAccountHistories");
                });

            modelBuilder.Entity("Shared.Models.UserEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ConnectionString")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Databasename")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<byte[]>("Image")
                        .HasColumnType("bytea");

                    b.Property<DateTime?>("LastModifiedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("ParentUserId")
                        .HasColumnType("integer");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PictureUrl")
                        .HasColumnType("text");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.Property<string>("ServerAddress")
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("ValidUntil")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("isFirstTimeLogin")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("ParentUserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Shared.Models.BuyAndSellTransaction", b =>
                {
                    b.HasOne("Shared.Models.CustomerTransactionHistory", "BuyTransaction")
                        .WithMany()
                        .HasForeignKey("BuyTransactionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Shared.Models.CustomerAccount", "CurrencyExchangeAccount")
                        .WithMany()
                        .HasForeignKey("CurrencyExchangeAccountId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Shared.Models.CustomerTransactionHistory", "CustomerBuyTransaction")
                        .WithMany()
                        .HasForeignKey("CustomerBuyTransactionId");

                    b.HasOne("Shared.Models.CustomerAccount", "CustomerAccount")
                        .WithMany()
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Shared.Models.CustomerTransactionHistory", "CustomerSellTransaction")
                        .WithMany()
                        .HasForeignKey("CustomerSellTransactionId")
                        .HasConstraintName("FK_BuyAndSellTransactions_CustomerTransactionHistories_Custom~1");

                    b.HasOne("Shared.Models.CustomerTransactionHistory", "SellTransaction")
                        .WithMany()
                        .HasForeignKey("SellTransactionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Shared.Models.Currency.CurrencyEntity", "SourceCurrencyEntity")
                        .WithMany()
                        .HasForeignKey("SourceCurrencyId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Shared.Models.Currency.CurrencyEntity", "TargetCurrencyEntity")
                        .WithMany()
                        .HasForeignKey("TargetCurrencyId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("BuyTransaction");

                    b.Navigation("CurrencyExchangeAccount");

                    b.Navigation("CustomerAccount");

                    b.Navigation("CustomerBuyTransaction");

                    b.Navigation("CustomerSellTransaction");

                    b.Navigation("SellTransaction");

                    b.Navigation("SourceCurrencyEntity");

                    b.Navigation("TargetCurrencyEntity");
                });

            modelBuilder.Entity("Shared.Models.Currency.CurrencyExchangeRate", b =>
                {
                    b.HasOne("Shared.Models.Currency.CurrencyEntity", "BaseCurrency")
                        .WithMany("BaseCurrencyRates")
                        .HasForeignKey("BaseCurrencyId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Shared.Models.Currency.CurrencyEntity", "TargetCurrency")
                        .WithMany("TargetCurrencyRates")
                        .HasForeignKey("TargetCurrencyId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("BaseCurrency");

                    b.Navigation("TargetCurrency");
                });

            modelBuilder.Entity("Shared.Models.CustomerBalance", b =>
                {
                    b.HasOne("Shared.Models.Currency.CurrencyEntity", "CurrencyEntity")
                        .WithMany("CustomerBalances")
                        .HasForeignKey("CurrencyId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Shared.Models.CustomerAccount", "CustomerAccount")
                        .WithMany("CustomerBalances")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CurrencyEntity");

                    b.Navigation("CustomerAccount");
                });

            modelBuilder.Entity("Shared.Models.CustomerTransactionHistory", b =>
                {
                    b.HasOne("Shared.Models.Currency.CurrencyEntity", "CurrencyEntity")
                        .WithMany("CustomerTransactionHistories")
                        .HasForeignKey("CurrencyId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Shared.Models.CustomerAccount", "CustomerAccount")
                        .WithMany("CustomerTransactionHistories")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CurrencyEntity");

                    b.Navigation("CustomerAccount");
                });

            modelBuilder.Entity("Shared.Models.Helpers.UserPreference", b =>
                {
                    b.HasOne("Shared.Models.Currency.CurrencyEntity", "CurrencyEntity")
                        .WithOne("UserPreference")
                        .HasForeignKey("Shared.Models.Helpers.UserPreference", "BaseCurrencyId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("CurrencyEntity");
                });

            modelBuilder.Entity("Shared.Models.TransferBetweenAccountHistory", b =>
                {
                    b.HasOne("Shared.Models.CustomerAccount", "CommisionAccount")
                        .WithMany()
                        .HasForeignKey("CommisionAccountId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Shared.Models.Currency.CurrencyEntity", "CommisionCurrencyEntity")
                        .WithMany()
                        .HasForeignKey("CommisionCurrencyId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Shared.Models.Currency.CurrencyEntity", "CurrencyEntity")
                        .WithMany()
                        .HasForeignKey("CurrencyId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Shared.Models.CustomerAccount", "RecieverAccount")
                        .WithMany()
                        .HasForeignKey("RecieverId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Shared.Models.CustomerTransactionHistory", "RecieverTransactionHistory")
                        .WithMany()
                        .HasForeignKey("RecieverTransactionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Shared.Models.CustomerAccount", "SenederAccount")
                        .WithMany()
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Shared.Models.CustomerTransactionHistory", "SenderTransactionHistory")
                        .WithMany()
                        .HasForeignKey("SenderTransactionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_TransferBetweenAccountHistories_CustomerTransactionHistori~1");

                    b.HasOne("Shared.Models.CustomerTransactionHistory", "TransactionFeeAccount")
                        .WithMany()
                        .HasForeignKey("TransactionFeeAccountId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .HasConstraintName("FK_TransferBetweenAccountHistories_CustomerTransactionHistori~2");

                    b.Navigation("CommisionAccount");

                    b.Navigation("CommisionCurrencyEntity");

                    b.Navigation("CurrencyEntity");

                    b.Navigation("RecieverAccount");

                    b.Navigation("RecieverTransactionHistory");

                    b.Navigation("SenderTransactionHistory");

                    b.Navigation("SenederAccount");

                    b.Navigation("TransactionFeeAccount");
                });

            modelBuilder.Entity("Shared.Models.UserEntity", b =>
                {
                    b.HasOne("Shared.Models.UserEntity", "ParentUser")
                        .WithMany("SubUsers")
                        .HasForeignKey("ParentUserId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("ParentUser");
                });

            modelBuilder.Entity("Shared.Models.Currency.CurrencyEntity", b =>
                {
                    b.Navigation("BaseCurrencyRates");

                    b.Navigation("CustomerBalances");

                    b.Navigation("CustomerTransactionHistories");

                    b.Navigation("TargetCurrencyRates");

                    b.Navigation("UserPreference")
                        .IsRequired();
                });

            modelBuilder.Entity("Shared.Models.CustomerAccount", b =>
                {
                    b.Navigation("CustomerBalances");

                    b.Navigation("CustomerTransactionHistories");
                });

            modelBuilder.Entity("Shared.Models.UserEntity", b =>
                {
                    b.Navigation("SubUsers");
                });
#pragma warning restore 612, 618
        }
    }
}
