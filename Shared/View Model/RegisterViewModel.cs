using System.ComponentModel.DataAnnotations;

namespace Shared.View_Model
{
    public class RegisterViewModel : BaseInfoViewModel
    {
        [Required(ErrorMessage = "تایید رمز را وارد کنید."), Compare(nameof(Password),ErrorMessage = "رمز تایید مشابه به رمز اصلی نمی باشد.")]
        public string ConfirmPassword { get; set; }

        //[Required(ErrorMessage ="نام سرور ضروری است.")]
        //public string ServerName { get; set; }

    }
}
