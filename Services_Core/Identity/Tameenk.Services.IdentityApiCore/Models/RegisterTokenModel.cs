using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.IdentityApi.Models
{
    public class RegisterTokenModel
    {
        public string UserId { get; set; }
        public string Token { get; set; }
    }
}