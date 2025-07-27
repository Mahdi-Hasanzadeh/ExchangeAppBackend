using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Shared.Models.Currency
{
    public class CurrencyExchangeRate
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        
        //[JsonIgnore]
        //public UserEntity UserEntity { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "واحد ارز باید حداقل ۱ باشد.")]
        public int Unit { get; set; } = 1;  // واحد ارز (مثلاً یک دالر)

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Buy { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Sell { get; set; }

        public string? Source { get; set; }

        public string? Remark { get; set; }

        public DateTime EffectiveDate { get; set; } = DateTime.Now;  // When this rate was valid

        public DateTime? EndDate { get; set; }  // Nullable to indicate ongoing rate

        public bool IsActive { get; set; } = true;

        // Base currency: 1 dollar
        [JsonIgnore]
        public CurrencyEntity BaseCurrency { get; set; }

        [Required]
        public int BaseCurrencyId { get; set; }

        // Target Currency: 85 AFN
        [JsonIgnore]
        public CurrencyEntity TargetCurrency { get; set; }

        public int TargetCurrencyId { get; set; }

    }
}
