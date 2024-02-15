using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core.IVR
{
    public class RenewalSendLowestLinkSMSRequestModel
    {
        [JsonProperty("tkn")]
        public string Token { get; set; }

        [JsonProperty("PhoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("sendToCallerPhone")]
        public bool SendToCallerPhone { get; set; } = false;
    }
}
