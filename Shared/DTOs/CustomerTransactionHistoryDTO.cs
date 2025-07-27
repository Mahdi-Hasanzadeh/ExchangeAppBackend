using Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Models.Currency;

namespace Shared.DTOs
{
    public class CustomerTransactionHistoryDTO
    {
        public int TransactionId { get; set; }

        public int CustomerId { get; set; }

        public int UserId { get; set; }

        [Required(ErrorMessage = "مبلغ الزامی است.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "مقدار پول باید از صفر بیشتر باشد.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "ارز را انتخاب کنید.")]
        [Range(0.1, double.MaxValue, ErrorMessage = "ارز را انتخاب کنید.")]
        public int CurrencyId { get; set; }


        [Required(ErrorMessage = "این فیلد الزامی است")]
        public required string DepositOrWithdrawBy { get; set; }

        [Required(ErrorMessage = "این فیلد الزامی است")]

        public int DocumentNumber { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }


        //[Required(ErrorMessage = "تفصیلات الزامی است")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "لطفا نوعیت معامله را مشخص کنید")]
        [Column(TypeName = "nvarchar(30)")]
        public DealType DealType { get; set; }

        public TransactionType TransactionType { get; set; } = TransactionType.Normal;

        public decimal NewAmountToUpdateTransaction { get; set; }

        public decimal NewAmountToUpdateBalance { get; set; }

        public CustomerTransactionHistoryDTO DeepClone()
        {
            return new CustomerTransactionHistoryDTO
            {
                TransactionId = this.TransactionId,
                CustomerId = this.CustomerId,
                Amount = Math.Abs(this.Amount),
                CurrencyId = this.CurrencyId,
                DepositOrWithdrawBy = this.DepositOrWithdrawBy,
                DocumentNumber = this.DocumentNumber,
                CreatedDate = this.CreatedDate,
                UpdatedDate = this.UpdatedDate,
                Description = this.Description,
                DealType = this.DealType,
                NewAmountToUpdateTransaction = this.NewAmountToUpdateTransaction,
                TransactionType = this.TransactionType,
               
            };
        }
    }
}
