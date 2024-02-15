using Newtonsoft.Json;
using System;

namespace Tameenk.Services.IdentityApi.Models
{
    public class CaptchaResponseModel
    {
        public string Image { get; set; }
        public string Token { get; set; }
        public string Input { get; set; }
        public string Captcha { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int ExpiredInSeconds { get; set; }
    }
}