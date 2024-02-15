using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.Profile.Component
{
    public class LoginOutput
    {
        public bool? PhoneNumberConfirmed { get; set; } = null;
        public string UserId { get;  set; }
        public bool RememberMe { get;  set; }
        public string Email { get; set; }
        public bool IsCorporateUser { get; set; }
        public bool IsCorporateAdmin { get; set; }
        public string AccessToken { get; set; }
        public string AccessTokenGwt { get; set; }
        public int TokenExpiryDate { get; set; }

        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }
        public string PhoneNo { get; set; }

        [JsonProperty("fnar")]
        public string FullNameAr { get; set; }

        [JsonProperty("fnen")]
        public string FullNameEn { get; set; }

        [JsonProperty("otp")]
        public int OTP { get; set; }

        [JsonProperty("getBirthDate")]
        public bool GetBirthDate { get; set; } = false;

        [JsonProperty("phoneVerification")]
        public bool PhoneVerification { get; set; } = false;

        [JsonProperty("hashed")]
        public string Hashed { get; set; }

        [JsonProperty("displayNameAr")]
        public string DisplayNameAr { get; set; }

        [JsonProperty("displayNameEn")]
        public string DisplayNameEn { get; set; }

        [JsonProperty("isYakeenChecked")]
        public bool IsYakeenChecked { get; set; }

        [JsonProperty("tokenExpirationDate")]
        public DateTime TokenExpirationDate { get; set; }
    }
}