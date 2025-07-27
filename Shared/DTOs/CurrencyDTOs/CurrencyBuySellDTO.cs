using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.CurrencyDTOs
{
    public class CurrencyBuySellDTO
    {
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencyCode { get; set; }
        public decimal TotalBuy { get; set; }
        public decimal TotalSell { get; set; }
        public byte[] CurrencyFlag { get; set; }
    }

}
