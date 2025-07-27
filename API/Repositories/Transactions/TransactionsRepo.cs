using API.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Shared.Contract.Transactions;
using Shared.DTOs.TransactionsDTOs;

namespace API.Repositories.Transactions
{
    public class TransactionsRepo : ITransactions
    {
        private readonly AppDbContext _context;

        public TransactionsRepo(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<TransferSummaryDTO>> GetTransferTransactionsByCustomerIdAsync(int customerId, int userId)
        {
            try
            {
                var filteredTransactions = _context.TransferBetweenAccountHistories
                                            .Where(t => t.UserId == userId && t.SenderId == customerId)
                                            .AsNoTracking(); // Apply filter before joining
                if (!filteredTransactions.Any())
                {
                    return [];
                }

                var result = await (
                                    from t in filteredTransactions
                                    join s in _context.CustomerAccounts.AsNoTracking().Where(c => c.UserId == userId) on t.SenderId equals s.Id
                                    join r in _context.CustomerAccounts.AsNoTracking().Where(c => c.UserId == userId) on t.RecieverId equals r.Id
                                    join commisionAccountGroup in _context.CustomerAccounts.AsNoTracking()
                                        .Where(c => c.UserId == userId)
                                        on t.CommisionAccountId equals commisionAccountGroup.Id into commisionAccountLeftJoin
                                    from commisionAccount in commisionAccountLeftJoin.DefaultIfEmpty() // Left Join

                                    join c in _context.Currencies.AsNoTracking().Where(c => c.UserId == userId)
                                        on t.CurrencyId equals c.CurrencyId into currencyGroup
                                    from currency in currencyGroup.DefaultIfEmpty() // Left join to prevent missing data issues

                                    join a in _context.Currencies.AsNoTracking().Where(c => c.UserId == userId)
                                        on t.CommisionCurrencyId equals a.CurrencyId into commisionCurrencyGroup
                                    from a in commisionCurrencyGroup.DefaultIfEmpty() // Left Join for CommisionCurrencyId

                                    where t.SenderId == customerId
                                    orderby t.CreatedDate descending
                                    select new TransferSummaryDTO
                                    {
                                        Id = t.Id,
                                        CommisionType = t.CommisionType,
                                        CommisionCurrencyId = t.CommisionCurrencyId,
                                        CommisionCurrencyName = a != null ? a.Name : "",
                                        SenderId = t.SenderId,
                                        SenderName = s.Name,
                                        RecieverId = t.RecieverId,
                                        RecieverName = r.Name,
                                        CurrencyId = t.CurrencyId,
                                        CreatedDate = t.CreatedDate,
                                        CurrencyName = currency != null ? currency.Name : "", // Handle missing currencies
                                        LastUpdatedDate = t.LastUpdatedDate,
                                        RecievedAmount = t.RecievedAmount,
                                        RecievedBy = t.RecievedBy,
                                        RecieverDescription = t.RecieverDescription,
                                        RecieverTransactionId = t.RecieverTransactionId,
                                        SendBy = t.SendBy,
                                        SendedAmount = t.SendedAmount,
                                        SenderDescription = t.SenderDescription,
                                        SenderTransactionId = t.SenderTransactionId,
                                        TransactionFeeAccountId = t.TransactionFeeAccountId ?? 0, // Handle null case
                                        TransactionFeeAmount = t.TransactionFeeAmount,
                                        TransactionFeeDescription = t.TransactionFeeDescription,
                                        TransactionFeeRecievedBy = t.TransactionFeeRecievedBy,
                                        CommisionAccountId = t.CommisionAccountId ?? 0, // Handle null case
                                        CommisionAccountName = commisionAccount != null ? commisionAccount.Name : ""
                                    }).ToListAsync();


                //var result = await (
                //    from t in filteredTransactions
                //    join s in _context.CustomerAccounts.AsNoTracking().Where(c => c.UserId == userId) on t.SenderId equals s.Id
                //    join r in _context.CustomerAccounts.AsNoTracking().Where(c => c.UserId == userId) on t.RecieverId equals r.Id
                //    join commisionAccount in _context.CustomerAccounts.AsNoTracking()
                //    .Where(c => c.UserId == userId) on t.CommisionAccountId equals commisionAccount.Id
                //    join c in _context.Currencies.AsNoTracking().Where(c => c.UserId == userId)
                //    on t.CurrencyId equals c.CurrencyId into currencyGroup
                //    from currency in currencyGroup.DefaultIfEmpty() // Left join to prevent missing data issues
                //    join a in _context.Currencies.AsNoTracking().Where(c => c.UserId == userId) on t.CommisionCurrencyId equals a.CurrencyId
                //    where t.SenderId == customerId
                //    orderby t.CreatedDate descending
                //    select new TransferSummaryDTO
                //    {
                //        Id = t.Id,
                //        CommisionType = t.CommisionType,
                //        CommisionCurrencyId = t.CommisionCurrencyId,
                //        CommisionCurrencyName = a.Name,
                //        SenderId = t.SenderId,
                //        SenderName = s.Name,
                //        RecieverId = t.RecieverId,
                //        RecieverName = r.Name,
                //        CurrencyId = t.CurrencyId,
                //        CreatedDate = t.CreatedDate,
                //        CurrencyName = currency != null ? currency.Name : "Unknown", // Handle missing currencies
                //        LastUpdatedDate = t.LastUpdatedDate,
                //        RecievedAmount = t.RecievedAmount,
                //        RecievedBy = t.RecievedBy,
                //        RecieverDescription = t.RecieverDescription,
                //        RecieverTransactionId = t.RecieverTransactionId,
                //        SendBy = t.SendBy,
                //        SendedAmount = t.SendedAmount,
                //        SenderDescription = t.SenderDescription,
                //        SenderTransactionId = t.SenderTransactionId,
                //        TransactionFeeAccountId = (int)t.TransactionFeeAccountId,
                //        TransactionFeeAmount = t.TransactionFeeAmount,
                //        TransactionFeeDescription = t.TransactionFeeDescription,
                //        TransactionFeeRecievedBy = t.TransactionFeeRecievedBy,
                //        CommisionAccountId = (int)t.CommisionAccountId,
                //        CommisionAccountName = commisionAccount.Name
                //    }
                //).ToListAsync();

                return result.Any() ? result : [];
            }
            catch (Exception ex)
            {
                return null; // Consider logging the error instead of returning null
            }
        }
    }
}
