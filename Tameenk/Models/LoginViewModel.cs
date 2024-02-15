using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tameenk.Models
{
    public class LoginViewModel
    {
        public string Email { get; set; }

        public string Password { get; set; }


        public bool IsValid
        {
            get
            {
                ModelErrors = new Dictionary<string, string>();
                if (string.IsNullOrEmpty(Email) )
                {
                    ModelErrors.Add("Email", "مطلوب*");
                    return false;

                }
                if(!IsValidEmail(Email))
                {
                    ModelErrors.Add("Email", "ايميل غير صحيح");
                    return false;
                }
                if (string.IsNullOrEmpty(Password))
                {
                    ModelErrors.Add("Password", "مطلوب*");
                    return false;
                }
                return true;
            }
           
        }
        public Dictionary<string,string> ModelErrors { get; set; }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public string Otp { get; set; }
        public string Lang { get; set; }
        public string Channel { get; set; }
        public string UserName { get; set; }
        public string PWD { get; set; }
    }
}