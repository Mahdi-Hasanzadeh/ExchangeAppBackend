using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Report
{
    public class CurrencyTransactionSummary
    {
        public int CurrencyId { get; set; }
        public string  CurrencyName { get; set; } = string.Empty;
        
        public byte[]? CurrencyImage { get; set; }

        public decimal TotalDeposits { get; set; }

        public decimal TotalWithdrawals { get; set; }

        public decimal Reminder { get; set; }
    }
}
