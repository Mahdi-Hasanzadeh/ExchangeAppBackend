using Shared.Enums;
using Shared.Models.Currency;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Shared.Models
{
    public class CustomerTransactionHistory
    {
        [Key]
        public int TransactionId { get; set; }

        public int UserId { get; set; }
        //[JsonIgnore]
        //public UserEntity UserEntity { get; set; }

        public int CustomerId { get; set; }

        //Navigation Property
        [JsonIgnore]
        public CustomerAccount CustomerAccount { get; set; }

        [Required(ErrorMessage = "مبلغ الزامی است.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "مقدار پول باید از صفر بیشتر باشد.")]
        public decimal Amount { get; set; }

        public int CurrencyId { get; set; }
        [JsonIgnore]
        public CurrencyEntity CurrencyEntity { get; set; }

        [Required(ErrorMessage = "این فیلد الزامی است")]
        public required string DepositOrWithdrawBy { get; set; }
        public int DocumentNumber { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? UpdatedDate { get; set; }


        //[Required(ErrorMessage = "تفصیلات الزامی است")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "لطفا نوعیت معامله را مشخص کنید")]
        [Column(TypeName = "tinyint")]
        public DealType DealType { get; set; }

        [Column(TypeName = "tinyint")]
        public TransactionType TransactionType { get; set; } = TransactionType.Normal;

        // RowVersion for concurrency control
        [Timestamp]
        public byte[] RowVersion { get; set; } = new byte[0];

    }
}
