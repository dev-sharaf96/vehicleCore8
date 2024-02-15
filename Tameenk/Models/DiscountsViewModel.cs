using System;using System.Collections.Generic;using System.Linq;using System.Web;using Tameenk.Services;namespace Tameenk.Models{
    public class DiscountsViewModel
    {
        public string DriverNIN { get; set; }
        public CaptchaModel Captcha { get; set; }
        public string Language { get; set; }
        public string ErrorMessage { get; set; }
        public ActivePolicyData ActivePolicyData { get; set; }
    }}