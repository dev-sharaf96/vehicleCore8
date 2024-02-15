using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Models
{
    public class AuthViewModel
    {
        public LoginViewModel Login { get; set; }

        public RegisterViewModel Register { get; set; }
    }
}