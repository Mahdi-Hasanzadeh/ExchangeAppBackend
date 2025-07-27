using Shared.Models.Currency;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.CurrencyDTOs
{
    public class CurrencyExchangeRateDTO
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "واحد ارز باید حداقل ۱ باشد.")]
        public int Unit { get; set; } = 1;  // واحد ارز (مثلاً یک دالر)

        [Required]
        public decimal Buy { get; set; }

        [Required]
        public decimal Sell { get; set; }

        public string? Source { get; set; }

        public string? Remark { get; set; }

        public DateTime EffectiveDate { get; set; } = DateTime.Now;  // When this rate was valid

        public DateTime? EndDate { get; set; }  // Nullable to indicate ongoing rate

        public bool IsActive { get; set; } = true;

        // Base currency: 1 dollar

        [Required]
        public int BaseCurrencyId { get; set; }

        // Target Currency: 85 AFN

        public int TargetCurrencyId { get; set; }
    }
}
