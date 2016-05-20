using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AnyCode
{
    public class ChangePwdSeatModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string Oldpassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "新密码和确认密码不一致")]
        public string ConfirmPassword { get; set; }
    }
}