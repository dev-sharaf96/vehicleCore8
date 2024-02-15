using Newtonsoft.Json;
using System;

namespace Tameenk.Services.Profile.Component
{
    public class IdentityCaptchaToken
    {
        [JsonProperty("captcha")]
        public string Captcha { get; set; }

        [JsonProperty("expiryDate")]
        public DateTime ExpiryDate { get; set; }
    }
}