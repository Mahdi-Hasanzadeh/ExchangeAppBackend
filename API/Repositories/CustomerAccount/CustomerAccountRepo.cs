using API.DataContext;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Contract;
using Shared.Contract.CustomerAccount;
using Shared.DTOs;
using Shared.DTOs.CurrencyDTOs;
using Shared.DTOs.CustomerDTOs;
using Shared.DTOs.GeneralLedgerDTOs;
using Shared.DTOs.TransactionsDTOs;
using Shared.Enums;
using Shared.Models;
using Shared.Models.Currency;
using Shared.Models.Helpers;

namespace API.Repositories.CustomerAccount
{

    public class CustomerAccountRepo : ICustomerAccountRepo, IDisposable
    {
        private readonly AppDbContext _context;
        private readonly ICurrencyRepository _currencyRepository;

        public CustomerAccountRepo(AppDbContext context, ICurrencyRepository currencyRepository)
        {
            _context = context;
            _currencyRepository = currencyRepository;
        }

        public async Task<int?> GetTotalOfCustomerAsync(int userId)
        {
            try
            {
                var total = await _context.CustomerAccounts.AsNoTracking()
                    .Where(c => c.UserId == userId && c.AccountType == eAccountType.Regular).CountAsync();
                return total;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<int?> GetTotalOfTransactions(int userId)
        {
            try
            {
                var total = await _context.CustomerTransactionHistories.AsNoTracking()
                    .Where(c => c.UserId == userId).CountAsync();
                return total;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<bool> UpdateTransactionDetail(int userId, CustomerTransactionHistoryDTO transactionHistoryDTO)
        {
            try
            {
                int affectedRows = await _context.CustomerTransactionHistories
                      .Where(t => t.UserId == userId && t.TransactionId == transactionHistoryDTO.TransactionId).AsNoTracking()
                      .ExecuteUpdateAsync(setters =>
                                          setters
                  .SetProperty(t => t.CreatedDate, transactionHistoryDTO.CreatedDate)
                  .SetProperty(t => t.UpdatedDate, DateTime.Now)
                  .SetProperty(t => t.DocumentNumber, transactionHistoryDTO.DocumentNumber)
                  .SetProperty(t => t.Description, transactionHistoryDTO.Description)
                  .SetProperty(t => t.DepositOrWithdrawBy, transactionHistoryDTO.DepositOrWithdrawBy)
                  );
                if (affectedRows > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<IEnumerable<CustomerAccountSummaryDTO>> GetAllAccountOfAUserAsync(int userId)
        {
            var accounts = await _context.CustomerAccounts.
                Where(c => c.UserId == userId && c.AccountType != eAccountType.Regular)
                .AsNoTracking().ToListAsync();
            return accounts.ToCustomerAccountSummaryDTOs();
        }

        public async Task<IEnumerable<CustomerAccountSummaryDTO>> GetAllCustomersAsync(int userId)
        {
            var customers = await _context.CustomerAccounts
                .Where(c => c.UserId == userId && c.AccountType == eAccountType.Regular)
                .AsNoTracking().ToListAsync();
            return customers.ToCustomerAccountSummaryDTOs();
        }

        public async Task<IEnumerable<CustomerAccountDTO>> GetAllCustomersInfoAsync(int userId)
        {
            try
            {
                var customers = await _context.CustomerAccounts.Where(c => c.UserId == userId && c.AccountType == eAccountType.Regular)
                    .AsNoTracking().ToListAsync();
                return customers.ToCustomerAccountDTOs();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<IEnumerable<PersonalAccountDTO>> GetPersonalAccountOfUser(int userId)
        {
            try
            {
                var personalAccounts = await _context.CustomerAccounts
                .Where(account => account.UserId == userId && account.AccountType != eAccountType.Regular)
                .AsNoTracking().ToListAsync();

                if (personalAccounts.Count == 0)
                {
                    return [];
                }
                List<PersonalAccountDTO> personalAccountDTOs = [];
                foreach (var item in personalAccounts)
                {
                    personalAccountDTOs.Add(new PersonalAccountDTO()
                    {
                        Id = item.Id,
                        UserId = item.UserId,
                        AccountNumber = item.AccountNumber,
                        AccountType = item.AccountType,
                        BorrowAmount = item.BorrowAmount,
                        IsActive = item.IsActive,
                        CreatedDate = item.CreatedDate,
                        LastModifiedDate = item.LastModifiedDate,
                        Name = item.Name
                    });
                }
                return personalAccountDTOs;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<int> AddCustomerTransactionAsync(CustomerTransactionHistoryDTO transactionHistoryDTO)
        {
            try
            {
                var result = await _context.CustomerTransactionHistories.AddAsync(transactionHistoryDTO.ToCustomerTransactionHistory());
                await _context.SaveChangesAsync();
                return result.Entity.TransactionId;

            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<CustomerTransactionHistoryDTO>> GetAllTransactionsByCustomerId(int userId, int customerId)
        {
            var transactions = await _context.CustomerTransactionHistories.AsNoTracking().
                Where(c => c.UserId == userId && c.CustomerId == customerId).ToListAsync();
            if (transactions == null)
            {
                return null;
            }

            return transactions.ToCustomerTransactionHistories();
        }

        public async Task<IEnumerable<CustomerTransactionHistoryDTO>> FilterAllTransactionsByCustomerId(int userId,int customerId,int? currencyId,DateTime? fromDate,DateTime? toDate)
        {
            var transactions = await _context.CustomerTransactionHistories
                .AsNoTracking()
                .Where(transaction =>
                    transaction.UserId == userId &&
                    transaction.CustomerId == customerId &&
                    // ⬇️ Only apply currency filter if currencyId is not null and not 0
                    (!currencyId.HasValue || currencyId == 0 || transaction.CurrencyId == currencyId) &&
                    (fromDate == null || EF.Functions.DateDiffDay(fromDate.Value, transaction.CreatedDate) >= 0) &&
                    (toDate == null || EF.Functions.DateDiffDay(transaction.CreatedDate, toDate.Value) >= 0)
                )
                .ToListAsync();


            return transactions.ToCustomerTransactionHistories();
        }

        public async Task<IEnumerable<TransactionHistoryDTO>> GetDailyTransactions(int userId, DateTime? FromDate = null, DateTime? ToDate = null, int? currencyId = null)
        {
            try
            {
                var dailyTransactions = await (from transaction in _context.CustomerTransactionHistories.AsNoTracking()
                                               join account in _context.CustomerAccounts.AsNoTracking()
                                               on transaction.CustomerId equals account.Id
                                               join currencyEntity in _context.Currencies.AsNoTracking()
                                               on transaction.CurrencyId equals currencyEntity.CurrencyId
                                               where transaction.UserId == userId &&
                                                (currencyId == 0 || transaction.CurrencyId == currencyId) &&
                                                (FromDate == null || EF.Functions.DateDiffDay(FromDate.Value, transaction.CreatedDate) >= 0) &&
                                                (ToDate == null || EF.Functions.DateDiffDay(transaction.CreatedDate, ToDate.Value) >= 0)
                                               select new TransactionHistoryDTO()
                                               {
                                                   TransactionId = transaction.TransactionId,
                                                   CreatedDate = transaction.CreatedDate,
                                                   UserId = transaction.UserId,
                                                   Amount = transaction.Amount,
                                                   DealType = transaction.DealType,
                                                   DepositOrWithdrawBy = transaction.DepositOrWithdrawBy,
                                                   Description = transaction.Description,
                                                   TransactionType = transaction.TransactionType,
                                                   CurrencyId = currencyEntity.CurrencyId,
                                                   CurrencyImage = currencyEntity.Image,
                                                   CurrencyName = currencyEntity.Name,
                                                   CustomerId = transaction.CustomerId,
                                                   CustomerName = account.Name,
                                                   UpdatedDate = transaction.UpdatedDate
                                               }
                                         ).ToListAsync();
                return dailyTransactions == null ? dailyTransactions : dailyTransactions;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<int?> GetLastUsedAccountNumber(int userId)
        {
            try
            {
                var result = await _context.UserPreferences.FirstOrDefaultAsync(c => c.UserId == userId);
                if (result == null)
                {
                    return null;
                }
                return result.LastUsedAccountNumber;

            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> InitializeAccountNumberSequenceTable(int userId)
        {
            try
            {
                await _context.UserPreferences.AddAsync(new UserPreference() { LastUsedAccountNumber = 0, UserId = userId });
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateLastUsedAccountNumber(int lastUsedNumber, int userId)
        {
            try
            {
                var affectedRows = await _context.UserPreferences.Where(c => c.UserId == userId).ExecuteUpdateAsync(setters => setters.SetProperty(c => c.LastUsedAccountNumber, lastUsedNumber));
                if (affectedRows > 0)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<(bool, string)> UpdateBalanceAfterRemoveTransaction(int userId, int customerId, int currencyId, decimal amount, DealType dealType)
        {
            try
            {
                var balance = await _context.CustomerBalances.FirstOrDefaultAsync
                    (c => c.UserId == userId && c.CustomerId == customerId && c.CurrencyId == currencyId);

                if (balance == null)
                {
                    return (false, "حساب مشتری یافت نشد.");
                }
                else
                {
                    var absolutAmount = Math.Abs(amount);
                    balance.Balance = dealType == DealType.Deposit ?
                        balance.Balance - absolutAmount : balance.Balance + absolutAmount;
                    balance.UpdatedAt = DateTime.Now;
                    return (true, "حساب مشتری بروز رسانی شد.");
                }
            }
            catch
            {
                return (false, "حساب مشتری بروز رسانی نشد.");
            }
        }

        public async Task<(bool, string)> WithdrawByCustomerIdAsync(int userId, int customerId, int currencyId, decimal amount)
        {
            try
            {
                var balance = await _context.CustomerBalances.FirstOrDefaultAsync
                    (c => c.UserId == userId && c.CustomerId == customerId && c.CurrencyId == currencyId);


                //TODO: check if the user can borrow money

                // option 1: user can not borrow money
                //if (balance == null)
                //{
                //    return (false,"شما حسابی برای این ارز ندارید.");
                //}

                // option 2: user can borrow money based on his/her borrow amount
                if (balance == null)
                {
                    // create customer balance
                    var newBalance = new CustomerBalance()
                    {
                        CustomerId = customerId,
                        CurrencyId = currencyId,
                        Balance = -amount,
                        CreatedAt = DateTime.Now,
                        UserId = userId
                    };

                    await _context.CustomerBalances.AddAsync(newBalance);
                    return (true, "حساب مشتری ساخته شد.");
                }
                else
                {
                    balance.Balance -= amount;
                    balance.UpdatedAt = DateTime.Now;
                    return (true, "برداشت از حساب با موفقیت انجام شد.");
                }
            }
            catch
            {
                return (false, "برداشت از حساب با مشکل مواجه شد.");

            }
        }

        public async Task<bool> DepositeByCustomerIdAsync(int userId, int customerId, int currencyId, decimal amount)
        {
            try
            {
                var balance = await _context.CustomerBalances.FirstOrDefaultAsync
                    (c => c.UserId == userId && c.CustomerId == customerId && c.CurrencyId == currencyId);

                if (balance == null)
                {
                    // create customer balance
                    var newBalance = new CustomerBalance()
                    {
                        CustomerId = customerId,
                        CurrencyId = currencyId,
                        Balance = amount,
                        CreatedAt = DateTime.Now,
                        UserId = userId
                    };
                    await _context.CustomerBalances.AddAsync(newBalance);
                }
                else
                {
                    balance.Balance += Math.Abs(amount);
                    balance.UpdatedAt = DateTime.Now;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<decimal?> CalculateCustomerBalanceInUSD
            (int userId, int customerId, List<CurrencyDetailDTO> exchangeRates)
        {
            try
            {
                var balances = await _context.CustomerBalances
                .Where(c => c.UserId == userId && c.CustomerId == customerId)
                .ToListAsync();

                //if (balances == null || balances.Count == 0) // Check if result is null or empty
                //{
                //    return null;
                //}
                if (balances == null) // Check if result is null or empty
                {
                    return null;
                }

                decimal? totalBalance = 0;

                foreach (var balance in balances)
                {
                    var rate = exchangeRates
                          .FirstOrDefault(rate => rate.TargetCurrencyId == balance.CurrencyId);

                    if (balance.CurrencyId == rate.TargetCurrencyId
                        && balance.CurrencyId == rate.BaseCurrencyId
                        )
                    {
                        totalBalance += balance.Balance;
                    }
                    else if (balance.CurrencyId == rate.TargetCurrencyId)
                    {
                        totalBalance += balance.Balance / rate.BuyValue;
                    }

                }
                return totalBalance;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<decimal?> GetBorrowAmountOfACustomer(int userId, int customerId)
        {
            var result = await _context.CustomerAccounts
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Id == customerId);
            if (result == null)
            {
                return null;
            }

            return result.BorrowAmount;
        }

        public async Task<(bool, string, IEnumerable<CustomerBalanceDetailsDTO>?)>GetCustomerBalanceById(int userId, int customerId)
        {
            var balances = await _context.CustomerBalances
                .Where(c => c.UserId == userId && c.CustomerId == customerId)
                .ToListAsync();
            if (balances == null)
            {
                return (false, "حساب مشتری یافت نشد.", null);
            }
            if (balances.Count == 0)
            {
                return (true, "مشتری هیچ حسابی ندارد.", Enumerable.Empty<CustomerBalanceDetailsDTO>());
            }

            var customerInfo = await _context.CustomerAccounts
            .Where(c => c.UserId == userId && c.Id == customerId)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Lastname
            })
                .FirstOrDefaultAsync();


            //var customerInfo = await _context.CustomerAccounts
            //    .FirstOrDefaultAsync(c => c.UserId == userId && c.Id == customerId);

            if (customerInfo == null)
            {
                return (false, "مشتری یافت نشد.", null);
            }

            var currencies = await _currencyRepository.GetAllCurrenciesByUserId(userId);

            if (currencies == null || currencies.Count == 0)
            {
                return (false, "هیچ ارزی یافت نشد.", null);
            }

            var customerFullname = $"{customerInfo.Name} {customerInfo.Lastname}";

            var customerBalanceDetails = (from b in balances
                                          join c in currencies
                                          on b.CurrencyId equals c.CurrencyId
                                          select new CustomerBalanceDetailsDTO
                                          {
                                              Id = b.Id,
                                              CurrencyId = b.CurrencyId,
                                              Balance = b.Balance,
                                              CreatedAt = b.CreatedAt,
                                              CurrencyName = c.Name,
                                              CustomerId = customerId,
                                              UpdatedAt = b.UpdatedAt,
                                              CustomerName = customerFullname,
                                              Image = c.Image,
                                              ImageUrl = c.ImageURL,
                                              UserId = b.UserId
                                          }).ToList();

            return (true, string.Empty, customerBalanceDetails);
        }

        public async Task<IEnumerable<GeneralListDTO>> GetCustomerBalancesByUserIdAsync
            (int userId, int? currencyId = null, bool? isDeptor = null, string? customerName = null)
        {
            try
            {
                //if (currencyId == null && isDeptor == null && customerName == null)
                //{
                //    var result = await (from balance in _context.CustomerBalances.AsNoTracking()
                //                        join customer in _context.CustomerAccounts.AsNoTracking()
                //                        on balance.CustomerId equals customer.Id
                //                        join currency in _context.Currencies.AsNoTracking()
                //                        on balance.CurrencyId equals currency.CurrencyId
                //                        where balance.UserId == userId && customer.AccountType == eAccountType.Regular
                //                        orderby balance.CustomerId, balance.CurrencyId
                //                        select new GeneralListDTO()
                //                        {
                //                            CustomerId = customer.Id,
                //                            Fullname = $"{customer.Name} {customer.Lastname}",
                //                            Mobile = customer.Mobile,
                //                            AccountNumber = customer.AccountNumber,
                //                            Reminder = balance.Balance,
                //                            CurrencyId = balance.CurrencyId,
                //                            CurrencyName = currency.Name,
                //                            //CustomerStatus = balance.Balance > 0 ? "طلبکار" : balance.Balance == 0 ? "0" : "بدهکار",
                //                            CurrencyImage = currency.Image
                //                        }
                //                  ).ToListAsync();
                //    return result == null ? null : result;
                //}

                //if (currencyId != null && isDeptor == null && customerName == null)
                //{
                //    var result = await (from balance in _context.CustomerBalances.AsNoTracking()
                //                        join customer in _context.CustomerAccounts.AsNoTracking()
                //                        on balance.CustomerId equals customer.Id
                //                        join currency in _context.Currencies.AsNoTracking()
                //                        on balance.CurrencyId equals currency.CurrencyId
                //                        where balance.UserId == userId && customer.AccountType == eAccountType.Regular
                //                        && currency.CurrencyId == currencyId
                //                        orderby balance.CustomerId, balance.CurrencyId
                //                        select new GeneralListDTO()
                //                        {
                //                            CustomerId = customer.Id,
                //                            Fullname = $"{customer.Name} {customer.Lastname}",
                //                            Mobile = customer.Mobile,
                //                            AccountNumber = customer.AccountNumber,
                //                            Reminder = balance.Balance,
                //                            CurrencyId = balance.CurrencyId,
                //                            CurrencyName = currency.Name,
                //                            //CustomerStatus = balance.Balance > 0 ? "طلبکار" : balance.Balance == 0 ? "0" : "بدهکار",
                //                            CurrencyImage = currency.Image
                //                        }
                //                  ).ToListAsync();
                //    return result == null ? null : result;
                //}
                //else
                //{
                var result = await (from balance in _context.CustomerBalances.AsNoTracking()
                                    join customer in _context.CustomerAccounts.AsNoTracking()
                                    on balance.CustomerId equals customer.Id
                                    join currency in _context.Currencies.AsNoTracking()
                                    on balance.CurrencyId equals currency.CurrencyId
                                    where balance.UserId == userId
                                          && customer.AccountType == eAccountType.Regular
                                          && (currencyId == null || currency.CurrencyId == currencyId)
                                          && (customerName == null || customer.Name.Contains(customerName) ||
                                              customer.Lastname.Contains(customerName) ||
                                              (customer.Name + " " + customer.Lastname).Contains(customerName))
                                          && (isDeptor == null ||
                                               (isDeptor == true && balance.Balance < 0) ||
                                               (isDeptor == false && balance.Balance > 0))
                                    orderby balance.CustomerId, balance.CurrencyId
                                    select new GeneralListDTO()
                                    {
                                        CustomerId = customer.Id,
                                        Fullname = $"{customer.Name} {customer.Lastname}",
                                        Mobile = customer.Mobile,
                                        AccountNumber = customer.AccountNumber,
                                        Reminder = balance.Balance,
                                        CurrencyId = balance.CurrencyId,
                                        CurrencyName = currency.Name,
                                        CurrencyImage = currency.Image
                                    }).ToListAsync();
                return result == null ? null : result;
                //}

            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public async Task<CustomerAccountDTO> GetCustomerInfoByCustomerIdAsync(int userId, int customerId)
        {
            try
            {
                var customer = await _context.CustomerAccounts.AsNoTracking()
                    .FirstOrDefaultAsync(customer => customer.UserId == userId && customer.Id == customerId);
                if (customer == null)
                {
                    return null;
                }

                return customer.ToCustomerAccountDTO();

            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public async Task<CustomerDTO?> GetCustomerByIdAsync(int userId, int id)
        {
            try
            {
                var customer = await _context.CustomerAccounts
                    .FirstOrDefaultAsync(customer => customer.UserId == userId && customer.Id == id);
                if (customer == null)
                {
                    return null;
                }
                return new CustomerDTO(customer.Id, customer.UserId);

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> UpdateTransactionAmount(int userId, CustomerTransactionHistoryDTO transactionDTO)
        {
            try
            {
                int affectedRows = await _context.CustomerTransactionHistories
                    .Where(t => t.UserId == userId && t.TransactionId == transactionDTO.TransactionId)
                    .AsNoTracking()
                    .ExecuteUpdateAsync(setters => setters
                    .SetProperty(t => t.Amount, transactionDTO.NewAmountToUpdateTransaction)
                    .SetProperty(t => t.UpdatedDate, DateTime.Now)
                    .SetProperty(t => t.CreatedDate, transactionDTO.CreatedDate)
                    .SetProperty(t => t.DepositOrWithdrawBy, transactionDTO.DepositOrWithdrawBy)
                    .SetProperty(t => t.Description, transactionDTO.Description)
                    .SetProperty(t => t.DocumentNumber, transactionDTO.DocumentNumber
                    ));
                if (affectedRows > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> UpdateCustomerBalanceAmount(int userId, int customerId, int currencyId, decimal amount, DealType dealType)
        {
            try
            {
                int affectedRows = await _context.CustomerBalances
                    .Where(b => b.UserId == userId && b.CustomerId == customerId && b.CurrencyId == currencyId).AsNoTracking()
                    .ExecuteUpdateAsync(setters => setters
                    .SetProperty(b => b.Balance, b => dealType == DealType.Withdraw ? b.Balance - (Math.Abs(amount)) : b.Balance + (Math.Abs(amount)))
                    .SetProperty(b => b.UpdatedAt, DateTime.Now));
                if (affectedRows > 0)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> IsTreasuryAccountExist(int userId)
        {
            try
            {
                return await _context.CustomerAccounts
           .AnyAsync(account => account.UserId == userId && account.AccountType == eAccountType.Treasury);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Return the Id of the account if exist
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<int> IsCurrencyExchangeAccountExist(int userId)
        {
            try
            {
                var result = await _context.CustomerAccounts.Select(balance => new { balance.UserId, balance.AccountType, balance.Id })
                            .FirstOrDefaultAsync(account => account.UserId == userId && account.AccountType == eAccountType.CurrencyExchange);
                if (result == null)
                {
                    return 0;
                }

                return result.Id;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<bool> CreateTreasuryBalanceForCurrency(int currencyId, int accountId, int userId)
        {
            try
            {
                var added = await _context.CustomerBalances.AddAsync(new CustomerBalance()
                {
                    Balance = 0,
                    CreatedAt = DateTime.Now,
                    CurrencyId = currencyId,
                    CustomerId = accountId,
                    UserId = userId
                });
                if (added == null)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<int?> GetTreasureAccountId(int userId)
        {
            try
            {
                var result = await _context.CustomerAccounts
                     .Select(balance => new { balance.UserId, balance.AccountType, balance.Id })
            .FirstOrDefaultAsync(account => account.UserId == userId && account.AccountType == eAccountType.Treasury);
                if (result == null)
                {
                    return null;
                }
                return result.Id;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> UpdateTreasuryAccount(int userId, decimal amount, int currencyId, bool isDeposit)
        {
            try
            {
                var newAmount = Math.Abs(amount);
                var accountId = await GetTreasureAccountId(userId);
                if (accountId == null)
                {
                    return false;
                }

                var affectedRows = await _context.CustomerBalances.Where(b => b.UserId == userId &&
                          b.CustomerId == accountId &&
                          b.CurrencyId == currencyId)
                      .ExecuteUpdateAsync(setter => setter
                      .SetProperty(b => b.Balance, b => isDeposit ? b.Balance + newAmount : b.Balance - newAmount));

                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<(bool, string)> CheckTreasuryAccountBasedOnBorrowAmount(int userId)
        {
            try
            {

                var treasuryAccountId = await GetTreasureAccountId(userId);
                if (treasuryAccountId == null)
                {
                    return (false, "حساب صندوق یافت نشد.");
                }

                var baseCurrency = await _currencyRepository.GetBaseCurrency(userId);
                if (baseCurrency == null)
                {
                    return (false, "ارز پایه یافت نشد.");
                }
                var exchangeRates = await _currencyRepository
                    .GetAllCurrencyDetails(baseCurrency.CurrencyId, userId);
                if (exchangeRates == null)
                {
                    return (false, "نرخ تبادله ارز برای ارز پایه یافت نشد.");
                }

                var totalbalance = await
                    CalculateCustomerBalanceInUSD(userId, (int)treasuryAccountId, exchangeRates.ToList());

                if (totalbalance == null)
                {
                    return (false, "خطا در محاسبه بیلانس");
                }

                var borrowamount = await GetBorrowAmountOfACustomer(userId, (int)treasuryAccountId);

                if (borrowamount == null)
                {
                    return (false, "حساب صندوق یافت نشد.");
                }

                if (totalbalance < -(borrowamount))
                {
                    return (false, "حساب صندوق به سقف قرضه خود رسیده است.");
                }

                return (true, string.Empty);

            }
            catch (Exception ex)
            {
                return (false, " خطا در سرور،بررسی حساب صندوق با مشکل مواجه شد.");
            }
        }

        public async Task<IEnumerable<BuyAndSellTransactionDTO>> GetConvertedDTOById(int userId)
        {
            List<BuyAndSellTransaction> result = await _context.BuyAndSellTransactions.AsNoTracking()
                .Where(t => t.UserId == userId && t.BuyAndSellType == CurrencyBuyAndSellType.Cash).ToListAsync();

            if (result == null)
            {
                return null;
            }

            var List = new List<BuyAndSellTransactionDTO>();

            foreach (BuyAndSellTransaction item in result)
            {
                List.Add(item.ToBuyAndSellTransactionDTO());

            }
            return List;

        }

        public async Task<IEnumerable<BuyAndSellTransactionDTO>> GetAllBuyAndSellTransactionsAsync(int userId, int customerId)
        {
            var result = await _context.BuyAndSellTransactions.AsNoTracking()
                .Where(t => t.UserId == userId && t.CustomerId == customerId).ToListAsync();
            if (result == null)
            {
                return null;
            }
            return result.ToBuyAndSellTransactionDTO();
        }

        public async Task<List<CurrencyDepositWithdrawDTO>> CalculateDepositWithdrawPerCurrency(int userId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            // Default to today if dates are null
            DateTime today = DateTime.Today;
            fromDate ??= today;
            toDate ??= today.AddDays(1).AddTicks(-1);

            // Fetch transactions within date range for the user
            var transactions = await _context.CustomerTransactionHistories
                .AsNoTracking()
                .Include(t => t.CurrencyEntity) // Assuming you have navigation property to Currency
                .Where(t => t.UserId == userId
                            && t.TransactionType == TransactionType.Normal
                            && t.CreatedDate >= fromDate
                            && t.CreatedDate <= toDate)
                .ToListAsync();

            // Group by currency and calculate totals
            var result = transactions
                .GroupBy(t => new { t.CurrencyId, t.CurrencyEntity.Name, t.CurrencyEntity.Code })
                .Select(g => new CurrencyDepositWithdrawDTO
                {
                    CurrencyId = g.Key.CurrencyId,
                    CurrencyName = g.Key.Name,
                    CurrencyCode = g.Key.Code,
                    TotalDeposit = g.Where(t => t.DealType == DealType.Deposit).Sum(t => t.Amount),
                    TotalWithdraw = g.Where(t => t.DealType == DealType.Withdraw).Sum(t => t.Amount),
                    CurrencyFlag = g.First().CurrencyEntity.Image
                })
                .OrderBy(dto => dto.CurrencyId)  // Sort by CurrencyId ascending
                .ToList();



            return result;
        }
        
        public async Task<List<CurrencyBuySellDTO>> CalculateBuySellCurrencyPerCurrency(int userId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            // Default to today if dates are null
            DateTime today = DateTime.Today;
            fromDate ??= today;
            toDate ??= today.AddDays(1).AddTicks(-1);

            // Fetch transactions within date range for the user
            var transactions = await _context.BuyAndSellTransactions
                .AsNoTracking()
                .Include(t => t.SourceCurrencyEntity)
                .Include(t => t.TargetCurrencyEntity)
                .Where(t => t.UserId == userId
                            && t.CreatedDate >= fromDate
                            && t.CreatedDate <= toDate)
                .ToListAsync();

            // Dictionary to accumulate Buy/Sell per currency
            var currencyTotals = new Dictionary<int, CurrencyBuySellDTO>();

            foreach (var t in transactions)
            {
                bool isOwner = t.CustomerId == null;

                if (isOwner && t.TransactionType == TransactionType.Buy)
                {
                    // Owner buys SourceCurrency, sells TargetCurrency
                    AddToDictionary(currencyTotals, t.SourceCurrencyId, t.SourceCurrencyEntity, buyAmount: t.Amount, sellAmount: 0);
                    AddToDictionary(currencyTotals, t.TargetCurrencyId, t.TargetCurrencyEntity, buyAmount: 0, sellAmount: t.ConvertedAmount);
                }
                else if (isOwner && t.TransactionType == TransactionType.Sell)
                {
                    // Owner buys TargetCurrency, sells SourceCurrency
                    AddToDictionary(currencyTotals, t.TargetCurrencyId, t.TargetCurrencyEntity, buyAmount: t.ConvertedAmount, sellAmount: 0);
                    AddToDictionary(currencyTotals, t.SourceCurrencyId, t.SourceCurrencyEntity, buyAmount: 0, sellAmount: t.Amount);
                }
                else if (!isOwner && t.TransactionType == TransactionType.Buy)
                {
                    // Owner sells SourceCurrency, buys TargetCurrency
                    AddToDictionary(currencyTotals, t.SourceCurrencyId, t.SourceCurrencyEntity, buyAmount: 0, sellAmount: t.Amount);
                    AddToDictionary(currencyTotals, t.TargetCurrencyId, t.TargetCurrencyEntity, buyAmount: t.ConvertedAmount, sellAmount: 0);
                }
                else if (!isOwner && t.TransactionType == TransactionType.Sell)
                {
                    // Owner buys SourceCurrency, sells TargetCurrency
                    AddToDictionary(currencyTotals, t.SourceCurrencyId, t.SourceCurrencyEntity, buyAmount: t.Amount, sellAmount: 0);
                    AddToDictionary(currencyTotals, t.TargetCurrencyId, t.TargetCurrencyEntity, buyAmount: 0, sellAmount: t.ConvertedAmount);
                }
            }

            // Return as list
            return currencyTotals.Values
                .OrderBy(c => c.CurrencyId)
                .ToList();
        }

        // Helper method to update dictionary
        private void AddToDictionary(Dictionary<int, CurrencyBuySellDTO> dict, int currencyId, CurrencyEntity currencyEntity, decimal buyAmount, decimal sellAmount)
        {
            if (!dict.TryGetValue(currencyId, out var dto))
            {
                dto = new CurrencyBuySellDTO
                {
                    CurrencyId = currencyId,
                    CurrencyName = currencyEntity.Name,
                    CurrencyCode = currencyEntity.Code,
                    CurrencyFlag = currencyEntity.Image,
                    TotalBuy = 0,
                    TotalSell = 0
                };
                dict[currencyId] = dto;
            }

            dto.TotalBuy += buyAmount;
            dto.TotalSell += sellAmount;
        }

        public async Task<decimal> CalculateLossAndBenefitsOfUser(int userId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            // If dates are null, use today's date range (00:00 to 23:59:59)
            DateTime today = DateTime.Today;
            fromDate ??= today;
            toDate ??= today.AddDays(1).AddTicks(-1); // end of today

            // Fetch transactions within date range and for the user
            var transactions = await _context.CustomerTransactionHistories
                .AsNoTracking()
                .Where(t => t.UserId == userId
                            && t.TransactionType == TransactionType.Normal
                            && t.CreatedDate >= fromDate
                            && t.CreatedDate <= toDate)
                .ToListAsync();

            var cashCommision = await _context.TransferBetweenAccountHistories
                .AsNoTracking()
                .Where(t => t.UserId == userId
                        && t.CommisionType == CommisionType.Cash
                        && t.CreatedDate >= fromDate
                        && t.CreatedDate <= toDate)
                .ToListAsync();

            // Calculate total cash commission
            decimal totalCashCommission = cashCommision.Sum(t => t.TransactionFeeAmount);

            if (transactions == null || transactions.Count == 0)
            {
                return 0;
            }

            decimal totalDeposit = transactions
                .Where(t => t.DealType == DealType.Deposit)
                .Sum(t => t.Amount);

            decimal totalWithdraw = transactions
                .Where(t => t.DealType == DealType.Withdraw)
                .Sum(t => t.Amount);

            return totalDeposit - totalWithdraw;
        }

        public async Task<List<ProfitLossSummaryDTO>> CalculateProfitLossInAsync(int userId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            //    // Step 1: Get base currency (USD)
            //    var baseCurrency = await _currencyRepository.GetBaseCurrency(userId);
            //    if (baseCurrency == null)
            //        return null;

            //    int baseCurrencyId = baseCurrency.CurrencyId;

            //    // Step 2: Get exchange rates (buy/sell) from each currency to USD
            //    var currencyDetails = await _currencyRepository.GetAllCurrencyDetails(baseCurrencyId, userId);
            //    var rateDict = currencyDetails.ToDictionary(x => x.TargetCurrencyId, x => x);

            //    // Step 3: Get all per-currency deposits/withdrawals and buy/sell
            //    var depositWithdrawList = await CalculateDepositWithdrawPerCurrency(userId, fromDate, toDate);
            //    var buySellList = await CalculateBuySellCurrencyPerCurrency(userId, fromDate, toDate);

            //    decimal totalDepositUSD = 0;
            //    decimal totalWithdrawUSD = 0;
            //    decimal totalBuyUSD = 0;
            //    decimal totalSellUSD = 0;

            //    // Step 4: Convert deposit/withdraw amounts to USD
            //    foreach (var item in depositWithdrawList)
            //    {
            //        if (rateDict.TryGetValue(item.CurrencyId, out var rate))
            //        {
            //            totalDepositUSD += (decimal)(item.TotalDeposit * rate.BuyValue);      // Buying USD
            //            totalWithdrawUSD += (decimal)(item.TotalWithdraw * rate.SellValue);  // Selling USD
            //        }
            //    }

            //    // Step 5: Convert buy/sell amounts to USD
            //    foreach (var item in buySellList)
            //    {
            //        if (rateDict.TryGetValue(item.CurrencyId, out var rate))
            //        {
            //            totalBuyUSD += (decimal)(item.TotalBuy * rate.BuyValue);
            //            totalSellUSD += (decimal)(item.TotalSell * rate.SellValue);
            //        }
            //    }

            //    // Step 6: Calculate net values and total profit
            //    var netCash = totalDepositUSD - totalWithdrawUSD;
            //    var netTrade = totalSellUSD - totalBuyUSD;
            //    var totalProfitUSD = netCash + netTrade;

            //    return new ProfitLossSummaryDTO
            //    {
            //        TotalDepositUSD = totalDepositUSD,
            //        TotalWithdrawUSD = totalWithdrawUSD,
            //        TotalBuyUSD = totalBuyUSD,
            //        TotalSellUSD = totalSellUSD,
            //        NetCashUSD = netCash,
            //        NetTradeUSD = netTrade,
            //        TotalProfitUSD = totalProfitUSD
            //    };
            //




            // Step 1: Get deposit/withdraw and buy/sell data
            var depositWithdrawList = await CalculateDepositWithdrawPerCurrency(userId, fromDate, toDate);
            var buySellList = await CalculateBuySellCurrencyPerCurrency(userId, fromDate, toDate);

            // Step 2: Merge and calculate per currency
            var result = new List<ProfitLossSummaryDTO>();

            var allCurrencyIds = depositWithdrawList.Select(x => x.CurrencyId)
                .Union(buySellList.Select(x => x.CurrencyId))
                .Distinct();

            foreach (var currencyId in allCurrencyIds)
            {
                var depositWithdraw = depositWithdrawList.FirstOrDefault(x => x.CurrencyId == currencyId);
                var buySell = buySellList.FirstOrDefault(x => x.CurrencyId == currencyId);

                var dto = new ProfitLossSummaryDTO
                {
                    CurrencyId = currencyId,
                    CurrencyName = depositWithdraw?.CurrencyName ?? buySell?.CurrencyName ?? "",
                    CurrencyCode = depositWithdraw?.CurrencyCode ?? buySell?.CurrencyCode ?? "",

                    TotalDeposit = depositWithdraw?.TotalDeposit ?? 0,
                    TotalWithdraw = depositWithdraw?.TotalWithdraw ?? 0,
                    TotalBuy = buySell?.TotalBuy ?? 0,
                    TotalSell = buySell?.TotalSell ?? 0
                };

                result.Add(dto);
            }

            return [.. result.OrderBy(c => c.CurrencyId)];




        }
    }
}
