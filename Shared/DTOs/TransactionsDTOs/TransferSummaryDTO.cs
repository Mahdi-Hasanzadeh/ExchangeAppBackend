using Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.TransactionsDTOs
{
    public class TransferSummaryDTO
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int SenderId { get; set; }

        public string SenderName { get; set; }

        public int RecieverId { get; set; }

        public string RecieverName { get; set; }

        public int? CommisionAccountId { get; set; }

        public string? CommisionAccountName { get; set; }

        public int RecieverTransactionId { get; set; }

        public int SenderTransactionId { get; set; }

        //حساب کارمزد تراکنش، یعنی کارمزد و هزینه این انتقال به کدام حساب واریز میشود.
        public int? TransactionFeeAccountId { get; set; }

        public decimal RecievedAmount { get; set; }

        public decimal TransactionFeeAmount { get; set; }

        public decimal SendedAmount { get; set; }

        public int CurrencyId { get; set; }

        public string CurrencyName { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? LastUpdatedDate { get; set; }

        public string SendBy { get; set; }

        public string RecievedBy { get; set; }

        public string TransactionFeeRecievedBy { get; set; }

        public string SenderDescription { get; set; }

        public string RecieverDescription { get; set; }

        public string TransactionFeeDescription { get; set; }

        public int? CommisionCurrencyId { get; set; }

        public CommisionType CommisionType { get; set; }

        public string CommisionCurrencyName { get; set; }

        public TransferBetweenAccountHistoryDTO DeepClone()
        {
            return new TransferBetweenAccountHistoryDTO()
            {
                Id = this.Id,
                CommisionType = this.CommisionType,
                CreatedDate = this.CreatedDate,
                CurrencyId = this.CurrencyId,
                LastUpdatedDate = this.LastUpdatedDate,
                RecievedAmount = this.RecievedAmount,
                RecievedBy = this.RecievedBy,
                RecieverDescription = this.RecieverDescription,
                RecieverId = this.RecieverId,
                RecieverTransactionId = this.RecieverTransactionId,
                SendBy = this.SendBy,
                SendedAmount = this.SendedAmount,
                SenderDescription = this.SenderDescription,
                SenderId = this.SenderId,
                SenderTransactionId = this.SenderTransactionId,
                TransactionFeeAccountId = this.TransactionFeeAccountId,
                TransactionFeeAmount = this.TransactionFeeAmount,
                TransactionFeeDescription = this.TransactionFeeDescription,
                TransactionFeeRecievedBy = this.TransactionFeeRecievedBy,
                CommisionAccountId = this.CommisionAccountId,
                CommisionCurrencyId = this.CommisionCurrencyId ?? 0, // ✅ FIXED: If null, set default value (0)
                UserId = this.UserId
            };
        }

    }
}
