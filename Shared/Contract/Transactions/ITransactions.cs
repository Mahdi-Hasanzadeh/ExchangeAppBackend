using Shared.DTOs.TransactionsDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contract.Transactions
{
    public interface ITransactions
    {
        public Task<IEnumerable<TransferSummaryDTO>> GetTransferTransactionsByCustomerIdAsync(int customerId,int userId);
    }
}
