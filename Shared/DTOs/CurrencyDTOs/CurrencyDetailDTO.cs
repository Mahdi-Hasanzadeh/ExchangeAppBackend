using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.CurrencyDTOs
{
    public class CurrencyDetailDTOForAllRates
    {
        public int TargetCurrencyId { get; set; }

        public int BaseCurrencyId { get; set; }

        public int Unit { get; set; }

        public int CurrencyExchangeRateId { get; set; }

        public int UserId { get; set; }
        public string Name { get; set; }

        public decimal? BuyValue { get; set; }

        public decimal? SellValue { get; set; }

    }
    public class CurrencyDetailDTO
    {
        [Required]
        //AFN
        public int TargetCurrencyId { get; set; }

        //USD
        [Required]
        public int BaseCurrencyId { get; set; }

        [Required]
        public int Unit { get; set; }
        public int CurrencyExchangeRateId { get; set; }

        [Required]
        public int UserId { get; set; }
        public string Name { get; set; }
        public string? ImageURL { get; set; }
        public byte[]? Image { get; set; }

        [Required(ErrorMessage = "مقدار خرید را وارد کنید")]
        [Range(-0.111, double.MaxValue, ErrorMessage = "مقدار خرید باید از صفر بیشتر باشد.")]
        public decimal? BuyValue { get; set; }

        [Required(ErrorMessage = "مقدار فروش را وارد کنید")]
        [Range(-0.111111, double.MaxValue, ErrorMessage = "مقدار فروش باید از صفر بیشتر باشد.")]
        public decimal? SellValue { get; set; }

        public string ImageString { get; set; }
    }
}
