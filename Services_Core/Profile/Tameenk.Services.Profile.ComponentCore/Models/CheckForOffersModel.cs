using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Dtos;

namespace Tameenk.Services.Profile.Component
{
    public class CheckForOffersModel
    {
        public string NIN { get; set; }
        public string Lang { get; set; }
        public string Channel { get; set; }

        [JsonProperty("hashed")]
        public string Hashed { get; set; }
        public ValidateCaptchaModel Captcha { get; set; }
    }
}
