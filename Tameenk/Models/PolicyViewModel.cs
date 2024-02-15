using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Models
{
    public class PolicyViewModel
    {
        public PolicyViewModel()
        {
            PolicyOutput = new Services.Checkout.Components.Output.PolicyOutput();
        }
        public string ButtonType { get; set; }
        public string SequenceNumber { get; set; }
        public string CustomCardNumber { get; set; }
        public string NIN { get; set; }
        public CaptchaModel Captcha { get; set; }
        public Tameenk.Services.Checkout.Components.Output.PolicyOutput PolicyOutput { get; set; }
    }


    public class CaptchaModel
    {
        public string Image { get; set; }
        public string Token { get; set; }
        public string Input { get; set; }
        public string Captcha { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int ExpiredInSeconds { get; set; }
    }
}