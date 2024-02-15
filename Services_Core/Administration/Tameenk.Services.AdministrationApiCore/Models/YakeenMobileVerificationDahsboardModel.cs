using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    public class YakeenMobileVerificationDahsboardModel
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("setYakeenmobileVerification")]
        public bool SetYakeenmobileVerification { get; set; }

        [JsonProperty("nationalId")]
        public string NationalId { get; set; }

        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }
    }
}