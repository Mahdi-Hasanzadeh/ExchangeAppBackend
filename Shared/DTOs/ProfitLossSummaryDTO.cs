using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class ProfitLossSummaryDTO
    {
        public int CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }

        public decimal TotalDeposit { get; set; }
        public decimal TotalWithdraw { get; set; }
        public decimal TotalBuy { get; set; }
        public decimal TotalSell { get; set; }

        public decimal NetCash => TotalDeposit - Math.Abs(TotalWithdraw);
        public decimal NetTrade => Math.Abs(TotalSell) - Math.Abs(TotalBuy);
        public decimal TotalProfitOrLoss => NetCash + NetTrade;
    }

}
