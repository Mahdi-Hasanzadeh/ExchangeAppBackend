using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.GeneralLedgerDTOs
{
    public class GeneralListDTO
    {
        public int CustomerId { get; set; }

        public string Fullname { get; set; } = string.Empty;

        public string Mobile { get; set; } = string.Empty;

        public int AccountNumber { get; set; }

        public int CurrencyId { get; set; }

        public string CurrencyName { get; set; } = string.Empty;

        public byte[]? CurrencyImage { get; set; } = null;

        public decimal Reminder { get; set; }

        //public string CustomerStatus { get; set; } = string.Empty;

    }
}
