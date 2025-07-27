using Shared.DTOs;
using Shared.DTOs.CurrencyDTOs;
using Shared.DTOs.CustomerDTOs;
using Shared.DTOs.TransactionsDTOs;
using Shared.Enums;
using Shared.Models;
using Shared.Models.Currency;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public static class ExtensionMethod
    {
        public static CustomerAccount ToCustomerAccountEntity(this CustomerAccountDTO customer)
        {
            return new CustomerAccount()
            {
                Name = customer.Firstname,
                Id = customer.Id,
                Lastname = customer.Lastname,
                AccountNumber = customer.AccountNumber,
                BorrowAmount = customer.BorrowAmount,
                CreatedDate = customer.CreatedDate,
                IDCardNumber = customer.IDCardNumber,
                Image = customer.Image,
                IsActive = customer.IsActive,
                Mobile = customer.Mobile,
                LastModifiedDate = customer.LastModifiedDate,
                PassportNumber = customer.PassportNumber,
                UserId = customer.UserId,
            };
        }

        public static CustomerAccountDTO ToCustomerAccountDTO(this CustomerAccount customer)
        {
            return new CustomerAccountDTO()
            {
                Firstname = customer.Name,
                Lastname = customer.Lastname,
                Id = customer.Id,
                AccountNumber = customer.AccountNumber,
                AccountType = customer.AccountType,
                BorrowAmount = customer.BorrowAmount,
                Fullname = $"{customer.Name} {customer.Lastname}",
                IDCardNumber = customer.IDCardNumber,
                PassportNumber = customer.PassportNumber,
                Mobile = customer.Mobile,
                UserId = customer.UserId,
                Image = customer.Image,
                IsActive = customer.IsActive,
                CreatedDate = customer.CreatedDate,
                LastModifiedDate = customer.LastModifiedDate,
            };
        }

        public static IEnumerable<CustomerAccountDTO> ToCustomerAccountDTOs(this IEnumerable<CustomerAccount> customers)
        {
            foreach (var customer in customers)
            {
                yield return new CustomerAccountDTO()
                {
                    Firstname = customer.Name,
                    Lastname = customer.Lastname,
                    Id = customer.Id,
                    AccountNumber = customer.AccountNumber,
                    AccountType = customer.AccountType,
                    BorrowAmount = customer.BorrowAmount,
                    Fullname = $"{customer.Name} {customer.Lastname}",
                    IDCardNumber = customer.IDCardNumber,
                    PassportNumber = customer.PassportNumber,
                    Mobile = customer.Mobile,
                    UserId = customer.UserId,
                    Image = customer.Image,
                    IsActive = customer.IsActive,
                    CreatedDate = customer.CreatedDate,
                    LastModifiedDate = customer.LastModifiedDate,
                };
            }
        }
        public static IEnumerable<BuyAndSellTransactionDTO> ToBuyAndSellTransactionDTO(this IEnumerable<BuyAndSellTransaction> transactions)
        {
            foreach (var transaction in transactions)
            {
                yield return new BuyAndSellTransactionDTO()
                {
                    Id = transaction.Id,
                    BuyTransactionId = transaction.BuyTransactionId,
                    SellTransactionId = transaction.SellTransactionId,
                    CurrencyExchangeAccountId = transaction.CurrencyExchangeAccountId,
                    Amount = transaction.Amount,
                    ConvertedAmount = transaction.ConvertedAmount,
                    Rate = transaction.Rate,
                    CreatedDate = transaction.CreatedDate,
                    UpdatedDate = transaction.UpdatedDate,
                    Description = transaction.Description,
                    SourceCurrencyId = transaction.SourceCurrencyId,
                    TargetCurrencyId = transaction.TargetCurrencyId,
                    TransactionType = transaction.TransactionType,
                    UserId = transaction.UserId,
                    CustomerId = transaction.CustomerId,
                    BuyAndSellType = transaction.BuyAndSellType,
                };
            }
        }

        public static BuyAndSellTransaction ToBuyAndSellTransaction(this BuyAndSellTransactionDTO transaction)
        {
            return new BuyAndSellTransaction()
            {
                Id = transaction.Id,
                BuyTransactionId = transaction.BuyTransactionId,
                SellTransactionId = transaction.SellTransactionId,
                CustomerBuyTransactionId = transaction.CustomerBuyTransactionId,
                CustomerSellTransactionId = transaction.CustomerSellTransactionId,
                CurrencyExchangeAccountId = transaction.CurrencyExchangeAccountId,
                Amount = transaction.Amount,
                ConvertedAmount = transaction.ConvertedAmount,
                Rate = transaction.Rate,
                CreatedDate = transaction.CreatedDate,
                UpdatedDate = transaction.UpdatedDate,
                Description = transaction.Description,
                SourceCurrencyId = transaction.SourceCurrencyId,
                TargetCurrencyId = transaction.TargetCurrencyId,
                TransactionType = transaction.TransactionType,
                UserId = transaction.UserId,
                BuyAndSellType = transaction.BuyAndSellType,
                CustomerId = transaction.CustomerId
            };
        }

        public static TransferSummaryDTO ToTransferSummaryDTO
            (this TransferBetweenAccountHistoryDTO transferDTO)
        {
            return new TransferSummaryDTO()
            {
                Id = transferDTO.Id,
                CreatedDate = transferDTO.CreatedDate,
                CurrencyId = transferDTO.CurrencyId,
                LastUpdatedDate = transferDTO.LastUpdatedDate,
                RecievedAmount = transferDTO.RecievedAmount,
                RecievedBy = transferDTO.RecievedBy,
                RecieverDescription = transferDTO.RecieverDescription,
                RecieverId = transferDTO.RecieverId,
                RecieverTransactionId = transferDTO.RecieverTransactionId,
                SendBy = transferDTO.SendBy,
                SendedAmount = transferDTO.SendedAmount,
                SenderDescription = transferDTO.SenderDescription,
                SenderId = transferDTO.SenderId,
                SenderTransactionId = transferDTO.SenderTransactionId,
                TransactionFeeAccountId = transferDTO.TransactionFeeAccountId,
                TransactionFeeAmount = transferDTO.TransactionFeeAmount,
                TransactionFeeDescription = transferDTO.TransactionFeeDescription,
                TransactionFeeRecievedBy = transferDTO.TransactionFeeRecievedBy,
                CommisionType = transferDTO.CommisionType,
                CommisionCurrencyId = transferDTO.CommisionCurrencyId,
                CommisionAccountId = transferDTO.CommisionAccountId,
            };
        }

        public static TransferBetweenAccountHistory ToTransferBetweenAccountHistory(this TransferBetweenAccountHistoryDTO transferDTO)
        {
            return new TransferBetweenAccountHistory()
            {
                Id = transferDTO.Id,
                UserId = transferDTO.UserId,
                CurrencyId = transferDTO.CurrencyId,
                RecieverTransactionId = transferDTO.RecieverTransactionId,
                RecieverId = transferDTO.RecieverId,
                RecievedAmount = transferDTO.RecievedAmount,
                RecievedBy = transferDTO.RecievedBy,
                RecieverDescription = transferDTO.RecieverDescription,
                SenderTransactionId = transferDTO.SenderTransactionId,
                SenderId = transferDTO.SenderId,
                SendedAmount = transferDTO.SendedAmount,
                SendBy = transferDTO.SendBy,
                SenderDescription = transferDTO.SenderDescription,
                TransactionFeeAccountId = transferDTO.TransactionFeeAccountId,
                TransactionFeeAmount = transferDTO.TransactionFeeAmount,
                TransactionFeeDescription = transferDTO.TransactionFeeDescription,
                TransactionFeeRecievedBy = transferDTO.TransactionFeeRecievedBy,
                CommisionCurrencyId = transferDTO.CommisionType == CommisionType.NoComission ? null : transferDTO.CommisionCurrencyId,
                CommisionType = transferDTO.CommisionType,
                CreatedDate = transferDTO.CreatedDate,
                LastUpdatedDate = transferDTO.LastUpdatedDate,
                CommisionAccountId = transferDTO.CommisionAccountId

            };
        }



        public static CurrencyDTO ToCurrencyDTO(this CurrencyEntity currencyEntity)
        {
            return new CurrencyDTO()
            {
                Code = currencyEntity.Code,
                CurrencyId = currencyEntity.CurrencyId,
                Image = currencyEntity.Image,
                ImageURL = currencyEntity.ImageURL,
                IsActive = currencyEntity.IsActive,
                Name = currencyEntity.Name,
                Symbol = currencyEntity.Symbol,
                Unit = currencyEntity.Unit,
                UserId = currencyEntity.UserId,
            };
        }
        public static CurrencyExchangeRateDTO ToCurrencyExchangeRateDTO
            (this CurrencyDetailDTO currencyDetailDTO)
        {
            return new CurrencyExchangeRateDTO()
            {
                BaseCurrencyId = currencyDetailDTO.BaseCurrencyId,
                Buy = (decimal)currencyDetailDTO.BuyValue!,
                Sell = (decimal)currencyDetailDTO?.SellValue!,
                TargetCurrencyId = currencyDetailDTO.TargetCurrencyId,
                Unit = currencyDetailDTO.Unit,
                UserId = currencyDetailDTO.UserId,
                Id = currencyDetailDTO.CurrencyExchangeRateId
            };
        }

        public static CurrencyExchangeRate ToCurrencyExchangeRate
            (this CurrencyExchangeRateDTO currencyExchangeRateDTO)
        {
            return new CurrencyExchangeRate()
            {
                BaseCurrencyId = currencyExchangeRateDTO.BaseCurrencyId,
                Buy = currencyExchangeRateDTO.Buy,
                Sell = currencyExchangeRateDTO.Sell,
                TargetCurrencyId = currencyExchangeRateDTO.TargetCurrencyId,
                UserId = currencyExchangeRateDTO.UserId,
                Unit = currencyExchangeRateDTO.Unit,
                EffectiveDate = currencyExchangeRateDTO.EffectiveDate,
                Remark = currencyExchangeRateDTO.Remark,
                Source = currencyExchangeRateDTO.Source,
            };
        }
        //public static string ConvertByteArrayToImage(this byte[] byteArray)
        //{
        //    // Check if it's an SVG image by checking for XML declaration
        //    string imageContent = System.Text.Encoding.UTF8.GetString(byteArray);
        //    if (imageContent.Contains("<?xml") || imageContent.Contains("<svg"))
        //    {
        //        // If it's SVG, return the data as SVG base64
        //        return $"data:image/svg+xml;base64,{Convert.ToBase64String(byteArray)}";
        //    }

        //    // Check for PNG by looking for the PNG signature (89 50 4E 47)
        //    if (byteArray.Length > 4 && byteArray[0] == 0x89 && byteArray[1] == 0x50 && byteArray[2] == 0x4E && byteArray[3] == 0x47)
        //    {
        //        // PNG image
        //        return $"data:image/png;base64,{Convert.ToBase64String(byteArray)}";
        //    }

        //    // Check for JPEG by looking for the JPEG signature (FF D8 FF)
        //    if (byteArray.Length > 3 && byteArray[0] == 0xFF && byteArray[1] == 0xD8 && byteArray[2] == 0xFF)
        //    {
        //        // JPEG image
        //        return $"data:image/jpeg;base64,{Convert.ToBase64String(byteArray)}";
        //    }

        //    // Default case for other images (or unknown formats)
        //    return $"data:image/octet-stream;base64,{Convert.ToBase64String(byteArray)}";
        //    //return $"data:image/svg;base64,{Convert.ToBase64String(byteArray)}";
        //}

        public static string ConvertByteArrayToImage(this byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0)
                return string.Empty;

            // SVG detection (text-based)
            string content = System.Text.Encoding.UTF8.GetString(byteArray);
            if (content.Contains("<?xml") || content.Contains("<svg"))
                return $"data:image/svg+xml;base64,{Convert.ToBase64String(byteArray)}";

            // PNG
            if (byteArray.Take(4).SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47 }))
                return $"data:image/png;base64,{Convert.ToBase64String(byteArray)}";

            // JPEG/JPG
            if (byteArray.Take(3).SequenceEqual(new byte[] { 0xFF, 0xD8, 0xFF }))
                return $"data:image/jpeg;base64,{Convert.ToBase64String(byteArray)}";

            // GIF
            if (byteArray.Take(3).SequenceEqual(new byte[] { 0x47, 0x49, 0x46 }))
                return $"data:image/gif;base64,{Convert.ToBase64String(byteArray)}";

            // WEBP
            if (byteArray.Length > 12 && byteArray[8] == 0x57 && byteArray[9] == 0x45 && byteArray[10] == 0x42 && byteArray[11] == 0x50)
                return $"data:image/webp;base64,{Convert.ToBase64String(byteArray)}";

            // BMP
            if (byteArray.Take(2).SequenceEqual(new byte[] { 0x42, 0x4D }))
                return $"data:image/bmp;base64,{Convert.ToBase64String(byteArray)}";

            // ICO
            if (byteArray.Take(4).SequenceEqual(new byte[] { 0x00, 0x00, 0x01, 0x00 }))
                return $"data:image/x-icon;base64,{Convert.ToBase64String(byteArray)}";

            // TIFF
            if (byteArray.Take(2).SequenceEqual(new byte[] { 0x49, 0x49 }) || byteArray.Take(2).SequenceEqual(new byte[] { 0x4D, 0x4D }))
                return $"data:image/tiff;base64,{Convert.ToBase64String(byteArray)}";

            // Unknown fallback
            return $"data:image/octet-stream;base64,{Convert.ToBase64String(byteArray)}";
        }

        public static int GetUserId(this ClaimsPrincipal user)
        {
            var userId = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User ID claim is missing.");
            return int.Parse(userId);
        }
        public static CurrencyEntity ToCurrencyEntity(this CurrencyDTO currencyDTO)
        {
            return new CurrencyEntity()
            {
                Code = currencyDTO.Code,
                CurrencyId = currencyDTO.CurrencyId,
                Image = currencyDTO.Image,
                ImageURL = currencyDTO.ImageURL,
                IsActive = currencyDTO.IsActive,
                Name = currencyDTO.Name,
                Symbol = currencyDTO.Symbol,
                UserId = currencyDTO.UserId,
                Unit = currencyDTO.Unit
            };
        }

        public static IEnumerable<CurrencyDTO> ToCurrencyDTOs
            (this IEnumerable<CurrencyEntity> currencyDTOs)
        {
            foreach (var currencyDTO in currencyDTOs)
            {
                yield return new CurrencyDTO()
                {
                    Code = currencyDTO.Code,
                    CurrencyId = currencyDTO.CurrencyId,
                    Image = currencyDTO.Image,
                    ImageURL = currencyDTO.ImageURL,
                    IsActive = currencyDTO.IsActive,
                    Name = currencyDTO.Name,
                    Symbol = currencyDTO.Symbol,
                    UserId = currencyDTO.UserId,
                    Unit = currencyDTO.Unit,
                };
            }
        }

        //public static CustomerBalanceDTO ToCustomerBalanceDTO(this CustomerBalance customerBalance)
        //{
        //    return new CustomerBalanceDTO()
        //    {
        //        Id = customerBalance.Id,
        //        CustomerId = customerBalance.CustomerId,
        //        AFN = customerBalance.AFN,
        //        EUR = customerBalance.EUR,
        //        IRR = customerBalance.IRR,
        //        PKR = customerBalance.PKR,
        //        RowVersion = customerBalance.RowVersion,
        //        TotalBalanceInUSD = customerBalance.TotalBalanceInUSD,
        //        USD = customerBalance.USD,
        //    };
        //}
        public static IEnumerable<CustomerAccountSummaryDTO> ToCustomerAccountSummaryDTOs
            (this IEnumerable<CustomerAccount> customerAccounts)
        {
            foreach (var customer in customerAccounts)
            {
                yield return new CustomerAccountSummaryDTO(customer.Id, customer.Name, customer.AccountNumber, customer.Lastname, (eAccountType)customer.AccountType);
            }
        }

        public static CustomerAccountSummaryDTO ToCustomerAccountSummaryDTO
            (this CustomerAccount customerAccount)
        {
            return new CustomerAccountSummaryDTO(customerAccount.Id, customerAccount.Name,
                customerAccount.AccountNumber, customerAccount.Lastname, (eAccountType)customerAccount.AccountType);
        }

        public static CustomerTransactionHistory ToCustomerTransactionHistory
            (this CustomerTransactionHistoryDTO dto)
        {
            return new CustomerTransactionHistory()
            {
                TransactionId = dto.TransactionId,
                CustomerId = dto.CustomerId,
                DepositOrWithdrawBy = dto.DepositOrWithdrawBy,
                Amount = dto.Amount,
                CreatedDate = dto.CreatedDate,
                CurrencyId = dto.CurrencyId,
                DealType = dto.DealType,
                Description = dto.Description,
                DocumentNumber = dto.DocumentNumber,
                TransactionType = dto.TransactionType,
                UserId = dto.UserId,
            };
        }

        public static IEnumerable<CustomerTransactionHistoryDTO> ToCustomerTransactionHistories
            (this IEnumerable<CustomerTransactionHistory>? transactionsDTO)
        {
            if (transactionsDTO != null)
            {
                foreach (var transaction in transactionsDTO)
                {
                    yield return new CustomerTransactionHistoryDTO()
                    {
                        TransactionId = transaction.TransactionId,
                        CustomerId = transaction.CustomerId,
                        CurrencyId = transaction.CurrencyId,
                        Amount = transaction.Amount,
                        DepositOrWithdrawBy = transaction.DepositOrWithdrawBy,
                        CreatedDate = transaction.CreatedDate,
                        DealType = transaction.DealType,
                        Description = transaction.Description,
                        DocumentNumber = transaction.DocumentNumber,
                        TransactionType = transaction.TransactionType,
                        UserId = transaction.UserId,
                    };
                }
            }
        }

        public static string ToThreeDigitFormatWithoutPointPersian(this int number)
        {
            CultureInfo persianCulture = new CultureInfo("fa-IR");
            string formattedNumber = number.ToString("N0", persianCulture); // "۱,۲۳۴,۵۶۷"
            return formattedNumber;
        }
        public static string ToFormattedDecimal(this decimal number)
        {
            // Round to 2 decimal places
            decimal rounded = Math.Round(number, 2, MidpointRounding.AwayFromZero);

            // If the number has no decimal places, return it as a whole number
            rounded = rounded % 1 == 0 ? Math.Truncate(rounded) : rounded;

            // Format with thousands separators and two decimal places if necessary
            return rounded.ToString("#,0.##");
        }

        public static string GetFormattedValue(this decimal number)
        {
            double rounded = Math.Round((double)number, 2, MidpointRounding.AwayFromZero);

            return rounded.ToString("#,0");
        }

        public static string GetFormattedAbsoluteValue(this decimal number)
        {

            if (number < 0)
            {
                number *= -1;
            }

            return GetFormattedValue(number);
        }



        public static decimal ToTwoDecimalPoints(this decimal number)
        {
            // Round to 2 decimal places
            decimal rounded = Math.Round(number, 2, MidpointRounding.AwayFromZero);

            // If the number has no decimal places, return it as a whole number
            return rounded % 1 == 0 ? Math.Truncate(rounded) : rounded;
        }
        public static string ToThreeDigitWithComma(this decimal number)
        {
            CultureInfo persianCulture = new CultureInfo("fa-IR");
            persianCulture.NumberFormat.NumberDecimalSeparator = ".";  // Set decimal separator to .
            persianCulture.NumberFormat.NumberGroupSeparator = ",";    // Set thousands separator to ,

            string formattedNumber = number.ToString(persianCulture);

            return formattedNumber;
        }
        public static string ToThreeDigitFormatPersian(this decimal number)
        {
            CultureInfo persianCulture = new CultureInfo("fa-IR");
            persianCulture.NumberFormat.NumberDecimalSeparator = ".";  // Set decimal separator to .
            persianCulture.NumberFormat.NumberGroupSeparator = ",";    // Set thousands separator to ,

            string formattedNumber = number.ToString("N2", persianCulture);

            return formattedNumber;
        }

        public static string ToThreeDigitFormatPersian(this decimal? number)
        {
            CultureInfo persianCulture = new CultureInfo("fa-IR");
            persianCulture.NumberFormat.NumberDecimalSeparator = ".";  // Set decimal separator to .
            persianCulture.NumberFormat.NumberGroupSeparator = ",";    // Set thousands separator to ,

            string formattedNumber = number?.ToString("N4", persianCulture)!;

            return formattedNumber;
        }

        public static string ToAfghanistanCalendarDate(this DateTime date)
        {
            PersianCalendar persianCalendar = new PersianCalendar();

            int year = persianCalendar.GetYear(date);
            int month = persianCalendar.GetMonth(date);
            int day = persianCalendar.GetDayOfMonth(date);
            int hour = persianCalendar.GetHour(date);
            int minute = persianCalendar.GetMinute(date);
            int second = persianCalendar.GetSecond(date);
            double millisecond = persianCalendar.GetMilliseconds(date);
            return $"{year}/{month:D2}/{day:D2} , {hour:D2}:{minute:D2}";
            //return $"{year}/{month:D2}/{day:D2}";
        }
        public static string ToAfghanistanCalendarDateOnly(this DateTime date)
        {
            PersianCalendar persianCalendar = new PersianCalendar();

            int year = persianCalendar.GetYear(date);
            int month = persianCalendar.GetMonth(date);
            int day = persianCalendar.GetDayOfMonth(date);
            return $"{year}/{month:D2}/{day:D2}";
            //return $"{year}/{month:D2}/{day:D2}";
        }
    }
}
