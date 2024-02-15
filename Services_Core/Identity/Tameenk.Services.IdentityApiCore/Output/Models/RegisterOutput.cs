using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.IdentityApi.Output
{
    public class RegisterOutput
    {
        public string UserId { get; set; }
        public string PhoneNumber { get; set; }
        public bool RememberMe { get; set; }
        public string Email { get; set; }
        public string AccessToken { get; set; }
    }
}