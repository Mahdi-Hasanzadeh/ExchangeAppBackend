using Shared.Enums;
using Shared.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Shared.DTOs.TransactionsDTOs
{
    public class BuyAndSellTransactionDTO
    {
        #region Property

        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public int CurrencyExchangeAccountId { get; set; }

        public int BuyTransactionId { get; set; }

        public int SellTransactionId { get; set; }

        public int? CustomerBuyTransactionId { get; set; }

        public int? CustomerSellTransactionId { get; set; }


        [Range(0.1, double.MaxValue, ErrorMessage = "مقدار پول را مشخص کنید.")]
        public decimal Amount { get; set; }

        [Required]
        [Range(0.1, double.MaxValue, ErrorMessage = "ارز مبدا را مشخص کنید.")]
        public int SourceCurrencyId { get; set; }

        public decimal Rate { get; set; }

        [Required]
        [Range(0.1, double.MaxValue, ErrorMessage = "ارز مقصد را مشخص کنید.")]
        public int TargetCurrencyId { get; set; }

        public decimal ConvertedAmount { get; set; }

        public TransactionType TransactionType { get; set; } = TransactionType.Buy;

        public string? Description { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? UpdatedDate { get; set; }

        public CurrencyBuyAndSellType BuyAndSellType { get; set; }

        public int? CustomerId { get; set; }

        #endregion

        #region Methods

        public BuyAndSellTransactionDTO DeepClone()
        {
            return new BuyAndSellTransactionDTO()
            {
                Id = this.Id,
                UserId = this.UserId,
                BuyTransactionId = this.BuyTransactionId,
                SellTransactionId = this.SellTransactionId,
                SourceCurrencyId = this.SourceCurrencyId,
                Rate = this.Rate,
                Amount = this.Amount,
                ConvertedAmount = this.ConvertedAmount,
                CurrencyExchangeAccountId = this.CurrencyExchangeAccountId,
                TargetCurrencyId = this.TargetCurrencyId,
                TransactionType = this.TransactionType,
                Description = this.Description,
                CreatedDate = this.CreatedDate,
                UpdatedDate = this.UpdatedDate,
                BuyAndSellType = this.BuyAndSellType,
                CustomerId = this.CustomerId,
            };
        }

        

        #endregion


    }
}
