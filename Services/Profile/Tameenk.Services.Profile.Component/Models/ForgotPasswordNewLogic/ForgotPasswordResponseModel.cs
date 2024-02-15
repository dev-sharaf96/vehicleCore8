using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.Profile.Component
{
    public class ForgotPasswordResponseModel
    {
        [JsonProperty("nationalIdExist")]
        public bool NationalIdExist { get; set; }

        [JsonProperty("uid")]
        public string UserId { get; set; }

        [JsonProperty("email")]
        public string MaskedEmail { get; set; }

        [JsonProperty("phone")]
        public string MaskedPhone { get; set; }

        [JsonProperty("hashed")]
        public string Hashed { get; set; }
    }
}