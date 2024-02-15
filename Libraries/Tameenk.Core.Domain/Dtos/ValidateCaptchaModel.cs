using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Dtos
{
    [JsonObject("validateCaptchaModel")]
    public class ValidateCaptchaModel
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("input")]
        public string Input { get; set; }
    }
}
