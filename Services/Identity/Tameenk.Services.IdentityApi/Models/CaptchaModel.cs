using Newtonsoft.Json;
using System;

namespace Tameenk.Services.IdentityApi.Models
{
    /// <summary>
    /// Captcha class.
    /// </summary>
    [JsonObject("captcha")]
    public class CaptchaModel
    {
        /// <summary>
        /// The captcha image.
        /// </summary>
        [JsonProperty("image")]
        public string Image { get; set; }

        /// <summary>
        /// Captcha token.
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; set; }

        /// <summary>
        /// Captcah expiration in seconds.
        /// </summary>
        [JsonProperty("expiredInSeconds")]
        public int ExpiredInSeconds { get; set; }
    }
}