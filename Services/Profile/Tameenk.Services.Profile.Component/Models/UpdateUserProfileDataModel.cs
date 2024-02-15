using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Profile.Component
{
    public class UpdateUserProfileDataModel
    {
        [JsonProperty("lang")]
        public string Lang { get; set; }

        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("updateInfoTypeId")]
        public int UpdateInfoTypeId { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("OTP")]
        public int? OTP { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
