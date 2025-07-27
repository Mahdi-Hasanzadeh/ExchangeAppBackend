using Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class TransactionHistoryDTO
    {

        public int TransactionId { get; set; }

        public int CustomerId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public int UserId { get; set; }

        public decimal Amount { get; set; }

        public int CurrencyId { get; set; }

        public string CurrencyName { get; set; } = string.Empty;

        public byte[]? CurrencyImage { get; set; }

        public string DepositOrWithdrawBy { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string Description { get; set; } = string.Empty;

        public DealType DealType { get; set; }

        public TransactionType TransactionType { get; set; }

    }


}
