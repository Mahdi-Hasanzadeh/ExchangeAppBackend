using Shared.Enums;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class PersonalAccountDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "لطفا نام حساب را وارد کنید")]
        [MaxLength(100, ErrorMessage = "نام حساب حداکثر باید 100 حرف داشته باشد.")]
        public string Name { get; set; }

        public int AccountNumber { get; set; }

        public decimal BorrowAmount { get; set; }

        [AllowedValues(
            eAccountType.Regular,
            eAccountType.Treasury,
            eAccountType.Incremental,
            eAccountType.Decremental,
            eAccountType.CurrencyExchange, ErrorMessage = "لطفا نوعیت حساب را مشخص کنید.")]
        public eAccountType? AccountType { get; set; }

        // Add UserId to link the customer to a specific user
        public int UserId { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? LastModifiedDate { get; set; }

    }
}
