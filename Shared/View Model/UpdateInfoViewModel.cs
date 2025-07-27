using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.View_Model
{
    public class UpdateInfoViewModel : BaseInfoViewModel
    {
        public int Id { get; set; }

        [Required(AllowEmptyStrings = true)]
        //[EmailAddress(ErrorMessage = "Please enter valid email address")]
        public string? Email { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string? PictureUrl { get; set; } = string.Empty;

    }
}
