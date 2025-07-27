using Shared.Models;
using Shared.Roles;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.DTOs.UserDTOs
{
    public class SubUserDTO
    {
        [Key]
        public int Id { get; set; }


        [Required(ErrorMessage = "نام کاربری الزامی است.")]
        public string Username { get; set; }

        [Required(ErrorMessage ="رمز عبور الزامی است.")]
        public string Password { get; set; }

        [Required]
        public UserRole Role { get; set; } = UserRole.User; // Default to User

        public string Email { get; set; } = string.Empty;

        public byte[]? Image { get; set; } = null;

        public int? ParentUserId { get; set; } // ParentUserId for sub-users (employees)

        public DateTime CreatedDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

    }
}
