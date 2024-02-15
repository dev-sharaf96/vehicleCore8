using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Services.Implementation.Policies;

namespace Tameenk.Models
{
    public class CheckDiscountViewModel
    {
        public CheckDiscountViewModel()
        {
            CheckDiscountOutput = new CheckDiscountOutput();
        }
        public string NIN { get; set; }
        public CaptchaModel Captcha { get; set; }
        public CheckDiscountOutput CheckDiscountOutput { get; set; }
    }
}