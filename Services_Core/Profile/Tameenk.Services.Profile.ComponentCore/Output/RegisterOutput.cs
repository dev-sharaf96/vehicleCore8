using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.Profile.Component
{
    public class RegisterOutput
    {
        public string UserId { get; set; }
        public string PhoneNumber { get; set; }
        public bool RememberMe { get; set; }
        public string AccessToken { get; set; }
        public string Email { get; set; }
        public string NationalId { get; set; }

        [JsonProperty("hashed")]
        public string Hashed { get; set; }

        [JsonProperty("fnar")]
        public string FullNameAr { get; set; }

        [JsonProperty("fnen")]
        public string FullNameEn { get; set; }

        [JsonProperty("getBirthDate")]
        public bool GetBirthDate { get; set; } = false;

        [JsonProperty("phoneVerification")]
        public bool PhoneVerification { get; set; } = false;

        [JsonProperty("otp")]
        public int OTP { get; set; }
    }
}