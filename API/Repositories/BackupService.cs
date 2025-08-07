using API.DataContext;
using Microsoft.EntityFrameworkCore;
using Shared.Contract;
using Shared.Enums;
using Shared.Models.Currency;
using Shared.Models.Helpers;
using Shared.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;

namespace API.Repositories
{
    public class BackupService : IBackupService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<BackupService> _logger;

        public BackupService(AppDbContext context, ILogger<BackupService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<dynamic> BackupUserDataAsync(int userId)
        {
            try
            {
                // Get all tables for the user
                var buyAndSellTransactions = await _context.BuyAndSellTransactions
                    .Where(b => b.UserId == userId)
                    .ToListAsync();

                var currencies = await _context.Currencies
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                var currencyExchangeRates = await _context.CurrencyExchangeRates
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                var customerAccounts = await _context.CustomerAccounts
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                var customerBalances = await _context.CustomerBalances
                    .Where(b => b.UserId == userId)
                    .ToListAsync();

                var customerTransactionHistories = await _context.CustomerTransactionHistories
                    .Where(b => b.UserId == userId)
                    .ToListAsync();

                var transferBetweenAccountHistories = await _context.TransferBetweenAccountHistories
                    .Where(b => b.UserId == userId)
                    .ToListAsync();

                var userPreferences = await _context.UserPreferences
                    .Where(b => b.UserId == userId)
                    .ToListAsync();

                var ownerInfos = await _context.OwnerInfos
                    .Where(b => b.UserId == userId)
                    .ToListAsync();

                // Create a backup object containing all user data
                var backupData = new
                {
                    BuyAndSellTransactions = buyAndSellTransactions,
                    Currencies = currencies,
                    CurrencyExchangeRates = currencyExchangeRates,
                    CustomerAccounts = customerAccounts,
                    CustomerBalances = customerBalances,
                    CustomerTransactionHistories = customerTransactionHistories,
                    TransferBetweenAccountHistories = transferBetweenAccountHistories,
                    UserPreferences = userPreferences,
                    OwnerInfos = ownerInfos
                };

                // Serialize the data to JSON
                //var jsonData = JsonConvert.SerializeObject(backupData,
                //    new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                var jsonData = System.Text.Json.JsonSerializer.Serialize(backupData, new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles, // Prevents reference loops
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, // Ignore null values
                    WriteIndented = true // (Optional) Pretty print JSON
                });

                return jsonData;
                //// Save the data to a file
                //await File.WriteAllTextAsync(backupFilePath, jsonData);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public class UserBackupData
        {
            public List<BuyAndSellTransaction> BuyAndSellTransactions { get; set; }
            public List<CurrencyEntity> Currencies { get; set; }
            public List<CurrencyExchangeRate> CurrencyExchangeRates { get; set; }
            public List<Shared.Models.CustomerAccount> CustomerAccounts { get; set; }
            public List<CustomerBalance> CustomerBalances { get; set; }
            public List<CustomerTransactionHistory> CustomerTransactionHistories { get; set; }
            public List<TransferBetweenAccountHistory> TransferBetweenAccountHistories { get; set; }
            public List<UserPreference> UserPreferences { get; set; }
            public List<OwnerInfo> OwnerInfos { get; set; }
        }

        public async Task<bool> RestoreUserDataAsync(int userId, string jsonData)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Step 1: Delete previous user data
                _context.BuyAndSellTransactions.RemoveRange(_context.BuyAndSellTransactions.Where(b => b.UserId == userId));
                _context.Currencies.RemoveRange(_context.Currencies.Where(c => c.UserId == userId));
                _context.CurrencyExchangeRates.RemoveRange(_context.CurrencyExchangeRates.Where(c => c.UserId == userId));
                _context.CustomerAccounts.RemoveRange(_context.CustomerAccounts.Where(c => c.UserId == userId));
                _context.CustomerBalances.RemoveRange(_context.CustomerBalances.Where(c => c.UserId == userId));
                _context.CustomerTransactionHistories.RemoveRange(_context.CustomerTransactionHistories.Where(c => c.UserId == userId));
                _context.TransferBetweenAccountHistories.RemoveRange(_context.TransferBetweenAccountHistories.Where(c => c.UserId == userId));
                _context.UserPreferences.RemoveRange(_context.UserPreferences.Where(c => c.UserId == userId));
                _context.OwnerInfos.RemoveRange(_context.OwnerInfos.Where(c => c.UserId == userId));

                await _context.SaveChangesAsync();

                // Step 2: Deserialize backup JSON
                var backupData = JsonSerializer.Deserialize<UserBackupData>(jsonData, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (backupData == null)
                    throw new Exception("Deserialization failed.");

                // Step 3: Insert with IDENTITY_INSERT ON
                async Task InsertWithIdentityInsert<T>(string tableName, List<T> data) where T : class
                {
                    if (data.Count == 0) return;

                    await _context.Database.ExecuteSqlRawAsync($"SET IDENTITY_INSERT [{tableName}] ON");
                    _context.Set<T>().AddRange(data);
                    await _context.SaveChangesAsync();
                    await _context.Database.ExecuteSqlRawAsync($"SET IDENTITY_INSERT [{tableName}] OFF");
                }

                await InsertWithIdentityInsert("Currencies", backupData.Currencies);
                await InsertWithIdentityInsert("CustomerAccounts", backupData.CustomerAccounts);
                await InsertWithIdentityInsert("OwnerInfos", backupData.OwnerInfos);
                await InsertWithIdentityInsert("UserPreferences", backupData.UserPreferences);
                await InsertWithIdentityInsert("CurrencyExchangeRates", backupData.CurrencyExchangeRates);
                await InsertWithIdentityInsert("CustomerBalances", backupData.CustomerBalances);
                await InsertWithIdentityInsert("CustomerTransactionHistories", backupData.CustomerTransactionHistories);
                await InsertWithIdentityInsert("BuyAndSellTransactions", backupData.BuyAndSellTransactions);
                await InsertWithIdentityInsert("TransferBetweenAccountHistories", backupData.TransferBetweenAccountHistories);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Log exception if needed
                return false;
            }

        }

    }
}
