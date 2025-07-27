using Shared.Enums;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.CustomerDTOs
{
    public class CustomerAccountDTO
    {
        #region Public Fields

        [Required(ErrorMessage = "لطفا نام مشتری را وارد کنید")]
        [MaxLength(100, ErrorMessage = "نام مشتری حداکثر باید 100 حرف داشته باشد.")]
        public string Firstname { get; set; }



        [MaxLength(100, ErrorMessage = "تخلص مشتری حداکثر باید 100 حرف داشته باشد.")]
        public string? Lastname { get; set; }



        [Range(0, double.MaxValue, ErrorMessage = "سقف اعتبار حداقل صفر است.")]
        public decimal BorrowAmount { get; set; }



        [RegularExpression("^(\\+93|0)\\d{9}$", ErrorMessage = "شماره موبایل درست نمی باشد.")]
        public string? Mobile { get; set; }



        public int Id { get; set; }

        public string Fullname { get; set; } = string.Empty;

        public int AccountNumber { get; set; }

        public string? PassportNumber { get; set; }

        public byte[]? Image { get; set; } = null;

        public string? IDCardNumber { get; set; }

        public byte[]? IdCardImage { get; set; }

        public string? RegistrationNumber { get; set; }

        public byte[]? RegistrationImage { get; set; }

        public byte[]? PassportImage { get; set; }

        public string? Address { get; set; }

        public eAccountType? AccountType { get; set; }

        public int UserId { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? LastModifiedDate { get; set; }

        #endregion

    }
}
