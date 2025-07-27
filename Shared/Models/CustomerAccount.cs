using Shared.DTOs.CustomerDTOs;
using Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Shared.Models
{
    public class CustomerAccount
    {

        #region Property

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        [MaxLength(100)]
        public string? Lastname { get; set; }

        [RegularExpression("^(\\+93|0)\\d{9}$")]
        public string? Mobile { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = new byte[0];

        [AllowedValues(nameof(eAccountType.Regular),
            nameof(eAccountType.Treasury),
            nameof(eAccountType.Incremental),
            nameof(eAccountType.CurrencyExchange),
            nameof(eAccountType.Decremental))]

        [JsonIgnore]
        public ICollection<CustomerTransactionHistory> CustomerTransactionHistories { get; set; } = new List<CustomerTransactionHistory>();

        [JsonIgnore]
        public ICollection<CustomerBalance> CustomerBalances { get; set; } = new List<CustomerBalance>();

        public eAccountType? AccountType { get; set; }

        public int AccountNumber { get; set; }

        public decimal BorrowAmount { get; set; }

        public string? Address { get; set; }

        public string? PassportNumber { get; set; }

        public byte[]? PassportImage { get; set; }

        public string? RegistrationNumber { get; set; }

        public byte[]? RegistrationImage { get; set; }

        public byte[]? Image { get; set; } = null;

        public string? IDCardNumber { get; set; }

        public byte[]? IdCardImage { get; set; }

        public int UserId { get; set; } // Add UserId to link the customer to a specific user

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? LastModifiedDate { get; set; }

        // Navigation property to the User class
        //[JsonIgnore]
        //public UserEntity User { get; set; }

        #endregion

        #region Methods

        public CustomerAccountDTO ToCustomerAccountDTO()
        {
            return new CustomerAccountDTO()
            {
                Id = Id,
                Firstname = Name,
                Lastname = Lastname,
                AccountNumber = AccountNumber,
                AccountType = AccountType,
                BorrowAmount = BorrowAmount,
                Fullname = $"{Name} {Lastname}",
                IDCardNumber = IDCardNumber,
                PassportNumber = PassportNumber,
                Mobile = Mobile,
                UserId = UserId,
                Image = Image,
                IsActive = IsActive,
                CreatedDate = CreatedDate,
                LastModifiedDate = LastModifiedDate,
                RegistrationNumber = RegistrationNumber,
                RegistrationImage = RegistrationImage,
                PassportImage = PassportImage,
                IdCardImage = IdCardImage,
                Address = Address,
            };
        }

        #endregion

    }


}
