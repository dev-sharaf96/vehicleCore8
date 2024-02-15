using System.ComponentModel.DataAnnotations;

namespace Tameenk.Core.Domain.Dtos
{
    public class LoginEndViewModel : BaseViewModel
    {
        //[Required]
        [EmailAddress]
        public string Email { get; set; }

        //[Required]
        public string Password { get; set; }

        public string UserName { get; set; }
        public string PWD { get; set; }
        public int OTP { get; set; }
    }
}
