using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.IdentityApi.Output
{
    public class LoginOutput
    {
        public bool PhoneNumberConfirmed { get; set; }
        public string UserId { get;  set; }
        public bool RememberMe { get;  set; }
        public string Email { get; set; }
        public bool IsCorporateUser { get; set; }
    }
}