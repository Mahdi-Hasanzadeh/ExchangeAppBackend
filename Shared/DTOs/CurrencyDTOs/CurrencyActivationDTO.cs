using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.CurrencyDTOs
{
    public class CurrencyActivationDTO
    {
        public int CurrencyId { get; set; }

        public bool IsActive { get; set; } = true;

        [Required]
        public int UserId { get; set; }
    }
}
