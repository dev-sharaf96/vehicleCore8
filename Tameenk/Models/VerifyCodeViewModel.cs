using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tameenk.Models
{
    public class VerifyCodeViewModel
    {
        public string UserId { get; set; }

        public string Code { get; set; }

        public string ReturnUrl { get; set; }

        public string PhoneNumber { get; set; }

        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }

        public bool IsValid
        {
            get
            {
                ModelErrors = new Dictionary<string, string>();
                if (string.IsNullOrEmpty(UserId))
                {
                    ModelErrors.Add("UserId", "مطلوب*");
                    return false;
                }
                if (string.IsNullOrEmpty(Code))
                {
                    ModelErrors.Add("Code", "مطلوب*");
                    return false;
                }
                if (string.IsNullOrEmpty(PhoneNumber))
                {
                    ModelErrors.Add("PhoneNumber", "مطلوب*");
                    return false;
                }

                return true;
            }

        }

        public Dictionary<string, string> ModelErrors { get; set; }

    }
}