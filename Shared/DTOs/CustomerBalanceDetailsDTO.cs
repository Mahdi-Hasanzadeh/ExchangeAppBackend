using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class CustomerBalanceDetailsDTO
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public string CustomerName { get; set; }

        public int CurrencyId { get; set; }

        public string CurrencyName { get; set; }

        public decimal Balance { get; set; }

        public byte[] Image { get; set; }

        public string ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int UserId { get; set; }

    }

}
