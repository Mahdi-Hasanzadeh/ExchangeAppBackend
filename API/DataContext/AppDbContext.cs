using Microsoft.EntityFrameworkCore;
using Shared.Models;
using Shared.Models.Helpers;
using Shared.Models.Currency;
using System.Collections.Generic;
using Shared.Enums;
using API.Services;


namespace API.DataContext
{
    public class AppDbContext : DbContext
    {
        //private readonly UserDatabaseService _userDbService;
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }
        //public AppDbContext(DbContextOptions<AppDbContext> options, UserDatabaseService userDbService = null) : base(options)
        //{
        //    _userDbService = userDbService;
        //}

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    var connectionString = _userDbService.GetConnectionString();
        //    if (!string.IsNullOrEmpty(connectionString) && _userDbService != null)
        //    {
        //        optionsBuilder.UseSqlServer(connectionString);
        //    }
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region UserEntity

            modelBuilder.Entity<UserEntity>()
                .HasOne(u => u.ParentUser)
                .WithMany(u => u.SubUsers)
                .HasForeignKey(u => u.ParentUserId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete/update

            #endregion

            #region OnwerInfo

            modelBuilder.Entity<OwnerInfo>()
            .HasIndex(e => e.UserId)
            .IsUnique(); // Only one logo per user

            #endregion

            #region TransferBetweenAccountHistory

            // For joins on SenderId/RecieverId
            modelBuilder.Entity<TransferBetweenAccountHistory>()
                .HasIndex(t => t.SenderId)
                .HasDatabaseName("IX_Transfers_SenderId");

            modelBuilder.Entity<TransferBetweenAccountHistory>()
                .HasIndex(t => t.RecieverId)
                .HasDatabaseName("IX_Transfers_RecieverId");

            // For currency lookups (if frequently filtered by CurrencyId)
            modelBuilder.Entity<TransferBetweenAccountHistory>()
                .HasIndex(t => t.CurrencyId)
                .HasDatabaseName("IX_Transfers_CurrencyId");


            modelBuilder.Entity<TransferBetweenAccountHistory>()
               .HasOne(b => b.CommisionCurrencyEntity)
               .WithMany()
               .HasForeignKey(b => b.CommisionCurrencyId)
               .OnDelete(DeleteBehavior.Restrict);


            //modelBuilder.Entity<TransferBetweenAccountHistory>()
            //   .HasOne(b => b.UserEntity)
            //   .WithMany()
            //   .HasForeignKey(b => b.UserId)
            //   .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TransferBetweenAccountHistory>()
                .HasOne(t => t.CommisionAccount)
                .WithMany()
                .HasForeignKey(t => t.CommisionAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TransferBetweenAccountHistory>()
                .HasOne(t => t.SenederAccount)
                .WithMany()
                .HasForeignKey(t => t.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TransferBetweenAccountHistory>()
                .HasOne(t => t.RecieverAccount)
                .WithMany()
                .HasForeignKey(t => t.RecieverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TransferBetweenAccountHistory>()
                .HasOne(t => t.RecieverTransactionHistory)
                .WithMany()
                .HasForeignKey(t => t.RecieverTransactionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TransferBetweenAccountHistory>()
                .HasOne(t => t.SenderTransactionHistory)
                .WithMany()
                .HasForeignKey(t => t.SenderTransactionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TransferBetweenAccountHistory>()
                .HasOne(t => t.TransactionFeeAccount)
                .WithMany()
                .HasForeignKey(t => t.TransactionFeeAccountId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<TransferBetweenAccountHistory>()
                .HasOne(t => t.CurrencyEntity)
                .WithMany()
                .HasForeignKey(t => t.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);



            #endregion

            #region CustomerTransactionHistory


            modelBuilder.Entity<CustomerTransactionHistory>()
                .Property(c => c.Amount)
                .HasPrecision(18, 4);  // 18 digits in total, 2 digits after the decimal point


            modelBuilder.Entity<CustomerTransactionHistory>()
                .HasOne(c => c.CustomerAccount)
                .WithMany(u => u.CustomerTransactionHistories)
                .HasForeignKey(c => c.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);



            // index on CustomerId + TransactionType
            modelBuilder.Entity<CustomerTransactionHistory>()
                 .HasIndex(e => new { e.CustomerId, e.TransactionType });


            modelBuilder.Entity<CustomerTransactionHistory>()
                .HasOne(cth => cth.CurrencyEntity)
                .WithMany(c => c.CustomerTransactionHistories)
                .HasForeignKey(cth => cth.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<CustomerTransactionHistory>()
            //    .HasOne(c => c.UserEntity)
            //    .WithMany()
            //    .HasForeignKey(cth => cth.UserId)
            //    .OnDelete(DeleteBehavior.Restrict);


            #endregion

            #region CustomerAccount


            // Define precision and scale for the BorrowAmount decimal column
            modelBuilder.Entity<CustomerAccount>()
                .Property(c => c.BorrowAmount)
                .HasPrecision(18, 4);  // 18 digits in total, 2 digits after the decimal point


            //Configure the relationship between UserEntity and CustomerAccount
            //modelBuilder.Entity<CustomerAccount>()
            //    .HasOne(c => c.User)
            //    //.WithMany(u => u.CustomerAccounts)
            //    //.HasForeignKey(c => c.UserId)
            //.OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<CustomerAccount>()
                .HasIndex(e => new { e.UserId, e.AccountType });

            // index on UserId Column
            modelBuilder.Entity<CustomerAccount>()
                .HasIndex(c => c.UserId);

            // Add a unique constraint on AccountNumber
            //modelBuilder.Entity<CustomerAccount>()
            //    .HasIndex(c => c.AccountNumber);

            modelBuilder.Entity<CustomerAccount>()
                    .HasIndex(c => new { c.UserId, c.AccountNumber })
                    .IsUnique(); // ✅ Ensures uniqueness per UserId


            modelBuilder.Entity<CustomerAccount>()
            .HasIndex(ca => new { ca.UserId, ca.AccountType });


            #endregion

            #region CurrencyExchangeRate

            modelBuilder.Entity<CurrencyExchangeRate>()
                .HasIndex(c => c.IsActive);


            modelBuilder.Entity<CurrencyExchangeRate>()
                .HasIndex(c => c.BaseCurrencyId);

            modelBuilder.Entity<CurrencyExchangeRate>()
                .HasIndex(c => c.EffectiveDate);

            modelBuilder.Entity<CurrencyExchangeRate>()
                .HasIndex(c => c.UserId);

            // modelBuilder.Entity<CurrencyExchangeRate>()
            //.HasOne(c => c.UserEntity)
            //.WithMany(e => e.CurrencyExchangeRates)
            //.HasForeignKey(e => e.UserId)
            //.OnDelete(DeleteBehavior.Cascade);


            // Relationship for BaseCurrency
            modelBuilder.Entity<CurrencyExchangeRate>()
                .HasOne(e => e.BaseCurrency)
                .WithMany(c => c.BaseCurrencyRates)
                .HasForeignKey(e => e.BaseCurrencyId)
                .OnDelete(DeleteBehavior.Restrict);  // Prevents cascading delete issues

            // Relationship for TargetCurrency
            modelBuilder.Entity<CurrencyExchangeRate>()
                .HasOne(e => e.TargetCurrency)
                .WithMany(c => c.TargetCurrencyRates)
                .HasForeignKey(e => e.TargetCurrencyId)
                .OnDelete(DeleteBehavior.Restrict);  // Prevents cascading delete issues


            #endregion

            #region CurrencyEntity

            modelBuilder.Entity<CurrencyEntity>()
                .HasIndex(c => new { c.UserId, c.CurrencyId });

            modelBuilder.Entity<CurrencyEntity>()
                .HasIndex(c => c.CurrencyId);

            //modelBuilder.Entity<CurrencyEntity>()
            // .HasOne(c => c.UserEntity)
            // .WithMany(e => e.Currency)
            // .HasForeignKey(e => e.UserId)
            // .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region BuyAndSellTransactions

            //modelBuilder.Entity<BuyAndSellTransaction>()
            //    .HasOne(u => u.UserEntity)
            //    .WithMany()
            //    .HasForeignKey(u => u.UserId)
            //    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BuyAndSellTransaction>()
                .HasOne(t => t.CustomerAccount)
                .WithMany()
                .HasForeignKey(t => t.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<BuyAndSellTransaction>()
                .HasOne(t => t.CurrencyExchangeAccount)
                .WithMany()
                .HasForeignKey(t => t.CurrencyExchangeAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BuyAndSellTransaction>()
                .HasOne(t => t.BuyTransaction)
                .WithMany()
                .HasForeignKey(t => t.BuyTransactionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BuyAndSellTransaction>()
                .HasOne(t => t.SellTransaction)
                .WithMany()
                .HasForeignKey(t => t.SellTransactionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BuyAndSellTransaction>()
                .HasOne(t => t.SourceCurrencyEntity)
                .WithMany()
                .HasForeignKey(t => t.SourceCurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BuyAndSellTransaction>()
                .HasOne(t => t.TargetCurrencyEntity)
                .WithMany()
                .HasForeignKey(t => t.TargetCurrencyId)
                .OnDelete(DeleteBehavior.Restrict);


            #endregion

            #region CustomerBalance

            modelBuilder.Entity<CustomerBalance>()
                .HasIndex(cb => new { cb.UserId, cb.CustomerId, cb.CurrencyId })
                .IncludeProperties(cb => cb.Balance);

            //modelBuilder.Entity<CustomerBalance>()
            //    .HasOne(b => b.UserEntity)
            //    .WithMany()
            //    .HasForeignKey(b => b.UserId)
            //    .OnDelete(DeleteBehavior.Restrict);

            // Define many-to-one relationship: Many CustomerBalances → One CurrencyEntity
            modelBuilder.Entity<CustomerBalance>()
                .HasOne(cb => cb.CurrencyEntity)      // CustomerBalance has one CurrencyEntity
                .WithMany(c => c.CustomerBalances) // CurrencyEntity has many CustomerBalances
                .HasForeignKey(cb => cb.CurrencyId) // Foreign key in CustomerBalance
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            // Customer → CustomerBalance (One-to-Many)
            modelBuilder.Entity<CustomerBalance>()
                .HasOne(cb => cb.CustomerAccount)   // Each CustomerBalance belongs to one Customer
                .WithMany(c => c.CustomerBalances) // A Customer can have many CustomerBalances
                .HasForeignKey(cb => cb.CustomerId) // Foreign key in CustomerBalance
                .OnDelete(DeleteBehavior.Cascade);  // If a Customer is deleted, delete their balances too



            #endregion

            #region UserPreference

            // modelBuilder.Entity<UserPreference>()
            //.HasOne(c => c.UserEntity)
            //.WithOne(c => c.UserPreference)
            //.HasForeignKey<UserPreference>(e => e.UserId)
            //.OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserPreference>()
           .HasOne(c => c.CurrencyEntity)
           .WithOne(c => c.UserPreference)
           .HasForeignKey<UserPreference>(e => e.BaseCurrencyId)
           .OnDelete(DeleteBehavior.Cascade);



            #endregion

            #region UserEntity



            #endregion

            #region IndexColumns

            //modelBuilder.Entity<CustomerBalance>()
            //.HasIndex(cb => new { cb.UserId, cb.CustomerId, cb.CurrencyId });

            #endregion
        }

        public DbSet<UserEntity> Users { get; set; }

        public DbSet<OwnerInfo> OwnerInfos { get; set; }

        public DbSet<CustomerAccount> CustomerAccounts { get; set; }

        public DbSet<CustomerTransactionHistory> CustomerTransactionHistories { get; set; }

        public DbSet<CustomerBalance> CustomerBalances { get; set; }

        public DbSet<UserPreference> UserPreferences { get; set; }

        public DbSet<CurrencyEntity> Currencies { get; set; }

        public DbSet<TransferBetweenAccountHistory> TransferBetweenAccountHistories { get; set; }

        public DbSet<CurrencyExchangeRate> CurrencyExchangeRates { get; set; }

        public DbSet<BuyAndSellTransaction> BuyAndSellTransactions { get; set; }
    }
}
