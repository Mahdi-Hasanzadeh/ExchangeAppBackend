using Shared.Models.Currency;
using Shared.Models.Helpers;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.CurrencyDTOs
{
    public class CurrencyDTO
    {

        public int CurrencyId { get; set; }

        [Required(ErrorMessage = "کد برای ارز الزامی است."),
        MaxLength(10, ErrorMessage = "کد حداکثر باید 10 حرف باشد")]
        public string Code { get; set; }  // e.g., USD, AFN, EUR

        [Required(ErrorMessage = "نام الزامی است."), MaxLength(50, ErrorMessage = "نام حداکثر باید 10 حرف باشد")]
        public string Name { get; set; }  // e.g., US Dollar, Afghan Afghani

        [MaxLength(5, ErrorMessage = "سمبول حداکثر باید 5 حرف باشد")]
        public string? Symbol { get; set; }  // e.g., $, ؋, €

        public bool IsActive { get; set; } = true;  // Enable/disable currencies

        [Required]
        public int UserId { get; set; }

        [Required(ErrorMessage = "واحد برای پول الزامی است.")]
        [Range(0.001, double.MaxValue, ErrorMessage = "مقدار واحد برای پول حداقل 1 است.")]
        
        public int Unit { get; set; }

        public byte[]? Image { get; set; } = null;

        public string? ImageURL { get; set; } = string.Empty;

    }
}
