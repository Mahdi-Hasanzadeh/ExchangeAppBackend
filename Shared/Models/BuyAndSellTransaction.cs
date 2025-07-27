using Shared.DTOs.TransactionsDTOs;
using Shared.Enums;
using Shared.Models.Currency;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Shared.Models
{
    public class BuyAndSellTransaction
    {
      
        #region Property

        [Key]
        public int Id { get; set; }



        [Required]
        public int UserId { get; set; }

        [Required]
        public int SourceCurrencyId { get; set; }



        [Column(TypeName = "decimal(18,4)")]
        public decimal Rate { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal ConvertedAmount { get; set; }



        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal Amount { get; set; }



        [JsonIgnore]
        public CustomerAccount CurrencyExchangeAccount { get; set; }
        
        [JsonIgnore]
        public CustomerTransactionHistory BuyTransaction { get; set; }

        [JsonIgnore]
        public CustomerTransactionHistory SellTransaction { get; set; }

        [JsonIgnore]
        public CustomerTransactionHistory CustomerBuyTransaction { get; set; }

        [JsonIgnore]
        public CustomerTransactionHistory CustomerSellTransaction { get; set; }
        
        [JsonIgnore]
        public CurrencyEntity SourceCurrencyEntity { get; set; }
        
        [JsonIgnore]
        public CurrencyEntity TargetCurrencyEntity { get; set; }

        [JsonIgnore]
        public CustomerAccount CustomerAccount { get; set; }



        [Timestamp] // RowVersion for concurrency control
        public byte[] RowVersion { get; set; } = new byte[0];



        public int CurrencyExchangeAccountId { get; set; }

        public int SellTransactionId { get; set; }

        public int? CustomerBuyTransactionId { get; set; }

        public int TargetCurrencyId { get; set; }

        public TransactionType TransactionType { get; set; }

        public CurrencyBuyAndSellType BuyAndSellType { get; set; } = 0;

        public int? CustomerId { get; set; }

        public int BuyTransactionId { get; set; }

        public int? CustomerSellTransactionId { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        //[JsonIgnore]
        //public UserEntity UserEntity { get; set; }


        #endregion

        #region Methods

        public BuyAndSellTransactionDTO ToBuyAndSellTransactionDTO()
        {
            return new BuyAndSellTransactionDTO()
            {
                Id = Id,
                BuyTransactionId = BuyTransactionId,
                SellTransactionId = SellTransactionId,
                CurrencyExchangeAccountId = CurrencyExchangeAccountId,
                Amount = Amount,
                ConvertedAmount = ConvertedAmount,
                Rate = Rate,
                CreatedDate = CreatedDate,
                UpdatedDate = UpdatedDate,
                Description = Description,
                SourceCurrencyId = SourceCurrencyId,
                TargetCurrencyId = TargetCurrencyId,
                TransactionType = TransactionType,
                UserId = UserId,
                CustomerId = CustomerId,
                BuyAndSellType = BuyAndSellType,

            };
        }

        #endregion

    }
}
