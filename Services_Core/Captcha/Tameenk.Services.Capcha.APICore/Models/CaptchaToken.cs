using Newtonsoft.Json;
using System;

namespace Tameenk.Services.Capcha.API
{
    public class CaptchaToken
    {
        [JsonProperty("captcha")]
        public string Captcha { get; set; }
        [JsonProperty("expiryDate")]
        public DateTime ExpiryDate { get; set; }
    }
}