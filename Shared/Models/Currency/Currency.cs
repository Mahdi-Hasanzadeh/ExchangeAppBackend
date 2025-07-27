using Shared.Models.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.Models.Currency
{
    public class CurrencyEntity
    {
        [Key]
        public int CurrencyId { get; set; }


        [Required, MaxLength(10)]
        public string Code { get; set; }  // e.g., USD, AFN, EUR

        [Required, MaxLength(50)]
        public string Name { get; set; }  // e.g., US Dollar, Afghan Afghani

        [MaxLength(5)]
        public string? Symbol { get; set; }  // e.g., $, ؋, €

        public bool IsActive { get; set; } = true;  // Enable/disable currencies

        public int UserId { get; set; }

        //[JsonIgnore]
        //public UserEntity UserEntity { get; set; }

        [JsonIgnore]
        public UserPreference UserPreference { get; set; }

        public byte[]? Image { get; set; }

        public string? ImageURL { get; set; }
        public int Unit { get; set; }

        [JsonIgnore]
        public ICollection<CustomerBalance> CustomerBalances { get; set; }

        [JsonIgnore]

        public ICollection<CustomerTransactionHistory> CustomerTransactionHistories { get; set; } = new List<CustomerTransactionHistory>();

        [JsonIgnore]

        public ICollection<CurrencyExchangeRate> BaseCurrencyRates { get; set; } = new List<CurrencyExchangeRate>();
        
        [JsonIgnore]
        public ICollection<CurrencyExchangeRate> TargetCurrencyRates { get; set; } = new List<CurrencyExchangeRate>();

    }
}
