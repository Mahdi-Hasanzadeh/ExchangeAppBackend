using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class OwnerInfo
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required(ErrorMessage ="نام صرافی الزامی است."),MaxLength(35,ErrorMessage ="نام صرافی حداکثر 35 کاراکتر است!")]
        public string OwnerName { get; set; } = "صرافی";

        public byte[]? Logo { get; set; }

        public string? LogoContentType { get; set; } // e.g. "image/png"
    }
}
