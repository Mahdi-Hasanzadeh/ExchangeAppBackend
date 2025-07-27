using Microsoft.AspNetCore.Mvc;
using Shared.Contract;
using Shared.Contract.CustomerAccount;
using Shared.Models;
using Shared.Models.Currency;
using System.Security.Claims;

namespace API
{
    public interface IAuthorizationService : IDisposable
    {
        Task<bool> IsUserOwnerOfCustomerAsync(int customerId, int userId);
        Task<bool> IsUserOwnerOfCurrency(int currencyId, int userId);
        Task<bool> IsUserOwnerOfTransaction(int transactionId, int userId);
        Task<bool> IsUserOwnerOfCurrencyExchangeRate(int currencyExchangeRateId, int userId);
    }

    public class AuthorizationService : IAuthorizationService
    {
        private readonly ICustomerAccountRepo _customerAccountRepo;
        private readonly IGenericRepository<CurrencyEntity> _currencyGenericRepository;
        private readonly IGenericRepository<CustomerTransactionHistory> _transactionGenericRepository;
        private readonly IGenericRepository<CurrencyExchangeRate> _genericCurrencyExchangeRate;

        public AuthorizationService(
            ICustomerAccountRepo customerAccountRepo,
            IGenericRepository<CurrencyEntity> currencyGenericRepository,
            IGenericRepository<CustomerTransactionHistory> transactionGenericRepository,
            IGenericRepository<CurrencyExchangeRate> genericCurrencyExchangeRate
            )
        {
            _customerAccountRepo = customerAccountRepo;
            _currencyGenericRepository = currencyGenericRepository;
            _transactionGenericRepository = transactionGenericRepository;
            _genericCurrencyExchangeRate = genericCurrencyExchangeRate;
        }

        public void Dispose()
        {
            _customerAccountRepo.Dispose();
        }

        public async Task<bool> IsUserOwnerOfCurrency(int currencyId, int userId)
        {
            var currency = await _currencyGenericRepository.GetByIdAsync(currencyId);

            if (currency == null)
            {
                return false;
            }

            return currency.UserId == userId; // Check if the logged-in user is the owner of the customer

        }

        public async Task<bool> IsUserOwnerOfCustomerAsync(int customerId, int userId)
        {
            var customer = await _customerAccountRepo.GetCustomerByIdAsync(userId, customerId);

            if (customer == null)
            {
                return false; // Customer not found
            }

            return customer.UserId == userId; // Check if the logged-in user is the owner of the customer
        }

        public async Task<bool> IsUserOwnerOfTransaction(int transactionId, int userId)
        {
            var transaction = await _transactionGenericRepository.GetByIdAsync(transactionId);

            if (transaction == null)
            {
                return false;
            }

            return await IsUserOwnerOfCustomerAsync(transaction.CustomerId, userId);
        }

        public async Task<bool> IsUserOwnerOfCurrencyExchangeRate(int currencyExchangeRateId, int userId)
        {
            var rate = await _genericCurrencyExchangeRate.GetByIdAsync(currencyExchangeRateId);

            if (rate == null)
            {
                return false;
            }

            return rate.UserId == userId;
        }

    }

}
