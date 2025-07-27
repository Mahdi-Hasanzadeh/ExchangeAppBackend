using API.DataContext;
using Microsoft.EntityFrameworkCore;
using Shared.Contract;
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

        public async Task RestoreUserDataAsync(int userId, string backupFilePath)
        {
            //try
            //{
            //    // Read the data from the backup file
            //    var jsonData = await File.ReadAllTextAsync(backupFilePath);

            //    // Try to deserialize the JSON data
            //    dynamic backupData;
            //    try
            //    {
            //        backupData = JsonConvert.DeserializeObject<dynamic>(jsonData);
            //    }
            //    catch (JsonException ex)
            //    {
            //        // Handle invalid JSON format
            //        _logger.LogError($"Invalid JSON format in backup file: {ex.Message}");
            //        return; // or throw a custom exception if you want to propagate the error
            //    }

            //    // Validate if required tables and fields exist in the backup data
            //    if (backupData?.BuyAndSellTransactions == null ||
            //        backupData?.Currencies == null ||
            //        backupData?.CustomerAccounts == null ||
            //        backupData?.CustomerBalances == null ||
            //        backupData?.CustomerTransactionHistories == null ||
            //        backupData?.TransferBetweenAccountHistories == null ||
            //        backupData?.UserPreferences == null ||
            //        backupData?.CurrencyExchangeRates == null
            //        )
            //    {
            //        _logger.LogError("Backup data is missing required tables.");
            //        return; // or throw a custom exception if you want to propagate the error
            //    }

            //    // Insert the data back into the database for the specific user
            //    await _context.BuyAndSellTransactions
            //        .AddRangeAsync(backupData.BuyAndSellTransactions);

            //    await _context.CurrencyExchangeRates
            //        .AddRangeAsync(backupData.CurrencyExchangeRates);

            //    await _context.Currencies
            //        .AddRangeAsync(backupData.Currencies);

            //    await _context.CustomerAccounts
            //        .AddRangeAsync(backupData.CustomerAccounts);

            //    await _context.CustomerBalances
            //        .AddRangeAsync(backupData.CustomerBalances);

            //    await _context.CustomerTransactionHistories
            //        .AddRangeAsync(backupData.CustomerTransactionHistories);

            //    await _context.TransferBetweenAccountHistories
            //        .AddRangeAsync(backupData.TransferBetweenAccountHistories);

            //    await _context.UserPreferences
            //        .AddRangeAsync(backupData.UserPreferences);

            //    // Save the changes to the database
            //    await _context.SaveChangesAsync();
            //}
            //catch (Exception ex)
            //{
            //    // Log the exception
            //    _logger.LogError($"An error occurred while restoring user data: {ex.Message}", ex);
            //}
        }

    }
}
