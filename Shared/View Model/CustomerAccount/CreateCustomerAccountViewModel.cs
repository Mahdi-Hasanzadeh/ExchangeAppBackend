using Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Shared.View_Model.CustomerAccount
{
    public class CreateCustomerAccountViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "لطفا نام مشتری را وارد کنید")]
        [MaxLength(100, ErrorMessage = "نام مشتری حداکثر باید 100 حرف داشته باشد.")]
        public required string Name { get; set; } = string.Empty;

        [MaxLength(100, ErrorMessage = "تخلص مشتری حداکثر باید 100 حرف داشته باشد.")]
        public string? Lastname { get; set; }

        public int AccountNumber { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "سقف اعتبار حداقل صفر است.")]
        public decimal BorrowAmount { get; set; }

        [RegularExpression("^(\\+93|0)\\d{9}$", ErrorMessage = "شماره موبایل درست نمی باشد.")]
        public string? Mobile { get; set; }

        [AllowedValues(eAccountType.Regular,
            eAccountType.Treasury,
            eAccountType.Incremental,
            eAccountType.Decremental,
            eAccountType.CurrencyExchange
            , ErrorMessage = "نوع حساب مشتری درست نیست.")]
        public eAccountType? AccountType { get; set; }

        // Add UserId to link the customer to a specific user
        public int UserId { get; set; }

        public string? PassportNumber { get; set; }

        public byte[]? Image { get; set; } = null;

        public string? IDCardNumber { get; set; }
    }
}
