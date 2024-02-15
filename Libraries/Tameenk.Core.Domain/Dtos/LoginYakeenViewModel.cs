using System.ComponentModel.DataAnnotations;

namespace Tameenk.Core.Domain.Dtos
{
    public class LoginYakeenViewModel : BaseViewModel
    {
        ////[Required]
        [EmailAddress]
        public string Email { get; set; }
        public string UserId { get; set; }
        public string Phone { get; set; }
        public string NationalId { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string PWD { get; set; }
    }
}
