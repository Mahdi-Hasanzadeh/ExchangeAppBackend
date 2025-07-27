using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs
{

    public class CustomerBalanceDTO
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public int CurrencyId { get; set; }

        public decimal Balance { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Balances in different currencies
        public decimal USD { get; set; } = 0;
        public decimal EUR { get; set; } = 0;
        public decimal AFN { get; set; } = 0;
        public decimal IRR { get; set; } = 0;
        public decimal PKR { get; set; } = 0;

        [Timestamp]
        public byte[] RowVersion { get; set; } = new byte[0];
    }
}
