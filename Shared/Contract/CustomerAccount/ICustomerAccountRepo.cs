using Shared.DTOs;
using Shared.DTOs.CurrencyDTOs;
using Shared.DTOs.CustomerDTOs;
using Shared.DTOs.GeneralLedgerDTOs;
using Shared.DTOs.TransactionsDTOs;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contract.CustomerAccount
{
    public interface ICustomerAccountRepo
    {
        public Task<List<CurrencyBuySellDTO>> CalculateBuySellCurrencyPerCurrency(int userId, DateTime? fromDate = null, DateTime? toDate = null);

        public Task<List<CurrencyDepositWithdrawDTO>> CalculateDepositWithdrawPerCurrency(int userId, DateTime? fromDate = null, DateTime? toDate = null);
        
        public Task<decimal> CalculateLossAndBenefitsOfUser(int userId, DateTime? fromDate = null, DateTime? ToDate = null);

        public Task<List<ProfitLossSummaryDTO>> CalculateProfitLossInAsync(int userId, DateTime? fromDate = null, DateTime? toDate = null);

        public Task<int?> GetTotalOfCustomerAsync(int userId);

        public Task<int?> GetTotalOfTransactions(int userId);

        public Task<CustomerAccountDTO> GetCustomerInfoByCustomerIdAsync(int userId, int customerId);

        public Task<CustomerDTO?> GetCustomerByIdAsync(int userId, int id);

        /// <summary>
        /// Update only text value of transaction and no rollback happened
        /// </summary>
        /// <param name="transactionHistoryDTO"></param>
        /// <returns>boolean</returns>
        public Task<bool> UpdateTransactionDetail(int userId, CustomerTransactionHistoryDTO transactionHistoryDTO);

        /// <summary>
        /// Update transaction amount
        /// </summary>
        /// <param></param>
        /// <returns>boolean</returns>
        public Task<bool> UpdateTransactionAmount(int userId, CustomerTransactionHistoryDTO transactionDTO);

        /// <summary>
        /// Return All customer account of a specific user(owner) 
        /// </summary>
        /// <returns>id and name of customers</returns>
        public Task<IEnumerable<CustomerAccountSummaryDTO>> GetAllCustomersAsync(int userId);

        public Task<IEnumerable<CustomerAccountSummaryDTO>> GetAllAccountOfAUserAsync(int userId);

        public Task<IEnumerable<CustomerAccountDTO>> GetAllCustomersInfoAsync(int userId);

        public Task<bool> IsTreasuryAccountExist(int userId);

        /// <summary>
        /// Return the Id of the account if exist
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<int> IsCurrencyExchangeAccountExist(int userId);

        public Task<bool> CreateTreasuryBalanceForCurrency(int currencyId, int accountId, int userId);

        public Task<int?> GetTreasureAccountId(int userId);

        public Task<IEnumerable<PersonalAccountDTO>> GetPersonalAccountOfUser(int userId);

        public Task<int> AddCustomerTransactionAsync(CustomerTransactionHistoryDTO transactionHistoryDTO);

        public Task<IEnumerable<CustomerTransactionHistoryDTO>> GetAllTransactionsByCustomerId(int userId, int customerId);

        public Task<IEnumerable<CustomerTransactionHistoryDTO>> FilterAllTransactionsByCustomerId(int userId, int customerId, int? currencyId, DateTime? fromDate, DateTime? toDate);

        public Task<IEnumerable<TransactionHistoryDTO>> GetDailyTransactions(int userId, DateTime? FromDate = null, DateTime? ToDate = null, int? currencyId = null);

        public Task<int?> GetLastUsedAccountNumber(int userId);

        public Task<bool> UpdateLastUsedAccountNumber(int lastUsedNumber, int userId);

        public Task<bool> InitializeAccountNumberSequenceTable(int userId);

        public Task<(bool, string)> UpdateBalanceAfterRemoveTransaction(int userId, int customerId, int currencyId, decimal amount, DealType dealType);

        public Task<bool> DepositeByCustomerIdAsync(int userId, int customerId, int currencyId, decimal amount);

        public Task<(bool, string)> WithdrawByCustomerIdAsync(int userId, int customerId, int currencyId, decimal amount);

        public Task<decimal?> CalculateCustomerBalanceInUSD(int userId, int customerId, List<CurrencyDetailDTO> exchangeRates);

        public Task<decimal?> GetBorrowAmountOfACustomer(int userId, int customerId);


        public Task<(bool, string, IEnumerable<CustomerBalanceDetailsDTO>?)>
            GetCustomerBalanceById(int userId, int customerId);

        public Task<IEnumerable<GeneralListDTO>> GetCustomerBalancesByUserIdAsync(int userId, int? currencyId = null, bool? isDeptor = null,string? customerName= null);

        public Task<bool> UpdateCustomerBalanceAmount(int userId, int customerId, int currencyId, decimal amount, DealType dealType);

        public Task SaveAsync();

        public void Dispose();

        public Task<bool> UpdateTreasuryAccount(int userId, decimal amount, int currencyId, bool isDeposit);

        public Task<(bool, string)> CheckTreasuryAccountBasedOnBorrowAmount(int userId);

        /// <summary>
        /// Get Cash Buy and Sell transactions of the app's owner
        /// </summary>
        /// <param name="userId">the ownerId</param>
        /// <returns></returns>
        public Task<IEnumerable<BuyAndSellTransactionDTO>> GetConvertedDTOById(int userId);

        /// <summary>
        /// Get Buy and Sell transactions of a customer (non-cash)
        /// </summary>
        /// <param name="userId">the ownerId</param>
        /// <param name="customerId">the customerId</param>
        /// <returns></returns>
        public Task<IEnumerable<BuyAndSellTransactionDTO>> GetAllBuyAndSellTransactionsAsync(int userId,int customerId);
    }
}
