using Shared.Models;
using Shared.Models.Currency;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Enums;

namespace Shared.DTOs.TransactionsDTOs
{
    public class TransferBetweenAccountHistoryDTO
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int? CommisionAccountId { get; set; }

        public int RecieverTransactionId { get; set; }

        public int SenderTransactionId { get; set; }

        public int CommisionCurrencyId { get; set; }

        public CommisionType CommisionType { get; set; } = CommisionType.NoComission;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? LastUpdatedDate { get; set; }

        public int? TransactionFeeAccountId { get; set; } //حساب کارمزد تراکنش، یعنی کارمزد و هزینه این انتقال به کدام حساب واریز میشود.

        public string? TransactionFeeDescription { get; set; } = string.Empty;




        [Range(0, double.MaxValue, ErrorMessage = "مقدار واریز را مشخص کنید.")]
        public decimal RecievedAmount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "مقدار مابه تفاوت حداقل صفر است.")]
        public decimal TransactionFeeAmount { get; set; }




        [Required(ErrorMessage = "توضیحات شخص فرستنده را مشخص کنید.")]
        public string SendBy { get; set; }

        [Required(ErrorMessage = "توضیحات شخص گیرنده را مشخص کنید.")]
        public string RecievedBy { get; set; }

        public string TransactionFeeRecievedBy { get; set; } = string.Empty;

        [Required(ErrorMessage = "تفصیلات شخص فرستنده را مشخص کنید.")]
        public string? SenderDescription { get; set; }

        [Required(ErrorMessage = "تفصیلات شخص گیرنده را مشخص کنید.")]
        public string? RecieverDescription { get; set; }




        [Required]
        [Range(0.1, double.MaxValue, ErrorMessage = "حساب فرستنده را مشخص کنید.")]
        public int SenderId { get; set; }

        [Required]
        [Range(0.1, double.MaxValue, ErrorMessage = "حساب گیرنده را مشخص کنید.")]
        public int RecieverId { get; set; }

       
        [Required(ErrorMessage = "مبلغ الزامی است.")]
        [Range(0.1, double.MaxValue, ErrorMessage = "مقدار واریز برای گیرنده را مشخص کنید.")]
        public decimal SendedAmount { get; set; }

        [Required(ErrorMessage = "ارز را مشخص کنید.")]
        [Range(0.1, double.MaxValue, ErrorMessage = "ارز را مشخص کنید.")]
        public int CurrencyId { get; set; }


        
        

    }
}
