using Shared.Roles;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.UserDTOs
{
    public class EditSubUserDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "نام کاربری الزامی است.")]
        public string Username { get; set; }


        public string? Email { get; set; }

        public UserRole Role { get; set; }

        public byte[]? Image { get; set; }

        public string? Password { get; set; }
    }

}
