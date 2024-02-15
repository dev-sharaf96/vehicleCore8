using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    public class LoginConfirmationCodeViewModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string CaptchaToken { get; set; }
        public string CaptchaInput { get; set; }
    }
}