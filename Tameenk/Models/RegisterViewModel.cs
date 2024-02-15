using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tameenk.Models
{
    public class RegisterViewModel
    {
      //  public string FullName { get; set; }

        public string Email { get; set; }

        public string ConfirmEmail { get; set; }

        public string Mobile { get; set; }

        public string Password { get; set; }

        public bool IsValid
        {
            get
            {
               /* if (string.IsNullOrEmpty(FullName))
                {
                    ModelErrors.Add("FullName", "مطلوب*");
                    return false;
                }*/
                if (string.IsNullOrEmpty(Email))
                {
                    ModelErrors.Add("Email", "مطلوب*");
                    return false;
                }
                if (string.IsNullOrEmpty(Mobile))
                {
                    ModelErrors.Add("Email", "مطلوب*");
                    return false;
                }
                if (string.IsNullOrEmpty(Password))
                {
                    ModelErrors.Add("Email", "مطلوب*");
                    return false;
                }
                return true;
            }
        }
        public Dictionary<string, string> ModelErrors { get; set; }
    }
}