using System.ComponentModel.DataAnnotations;

namespace Shared.View_Model
{
    public class BaseInfoViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا نام کاربری را وارد کنید.")]
        public string? Username { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا رمز را وارد کنید.")]
        public string? Password { get; set; }
    }
}
