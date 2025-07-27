using Shared.Enums;
using Shared.Models.Currency;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.Models
{

    public class TransferBetweenAccountHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        //[JsonIgnore]
        //public UserEntity UserEntity { get; set; }

        [Required]
        public int SenderId { get; set; }
        [JsonIgnore]
        public CustomerAccount SenederAccount { get; set; }

        [Required]
        public int RecieverId { get; set; }
        [JsonIgnore]
        public CustomerAccount RecieverAccount { get; set; }

        public int? CommisionAccountId { get; set; }
        [JsonIgnore]
        public CustomerAccount CommisionAccount { get; set; }

        public int RecieverTransactionId { get; set; }
        [JsonIgnore]
        public CustomerTransactionHistory RecieverTransactionHistory { get; set; }

        public int SenderTransactionId { get; set; }
        [JsonIgnore]
        public CustomerTransactionHistory SenderTransactionHistory { get; set; }

        //حساب کارمزد تراکنش، یعنی کارمزد و هزینه این انتقال به کدام حساب واریز میشود.
        public int? TransactionFeeAccountId { get; set; }
        [JsonIgnore]
        public CustomerTransactionHistory TransactionFeeAccount { get; set; }


        [Range(0.1, double.MaxValue, ErrorMessage = "مقدار واریز را مشخص کنید.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal RecievedAmount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "مقدار مابه تفاوت حداقل صفر است.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TransactionFeeAmount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "مقدار واریز برای گیرنده را مشخص کنید.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SendedAmount { get; set; }

        [Required(ErrorMessage = "ارز را مشخص کنید.")]
        [Range(0.1, double.MaxValue, ErrorMessage = "ارز را مشخص کنید.")]
        public int CurrencyId { get; set; }
        [JsonIgnore]
        public CurrencyEntity CurrencyEntity { get; set; }

        public int? CommisionCurrencyId { get; set; }

        [JsonIgnore]
        public CurrencyEntity CommisionCurrencyEntity { get; set; }

        public CommisionType CommisionType { get; set; } = CommisionType.NoComission;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? LastUpdatedDate { get; set; }

        [Required(ErrorMessage = "توضیحات شخص فرستنده را مشخص کنید.")]
        public string SendBy { get; set; }

        [Required(ErrorMessage = "توضیحات شخص گیرنده را مشخص کنید.")]
        public string RecievedBy { get; set; }

        [Required(ErrorMessage = "توضیحات حساب گیرنده مابه تفاوت را مشخص کنید.")]
        public string TransactionFeeRecievedBy { get; set; }

        public string? SenderDescription { get; set; }

        public string? RecieverDescription { get; set; }

        public string? TransactionFeeDescription { get; set; }

        public byte[] RowVersion { get; set; } = new byte[0];


    }
}
