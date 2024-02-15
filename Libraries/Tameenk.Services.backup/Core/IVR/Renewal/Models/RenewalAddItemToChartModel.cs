using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core.IVR
{
    public class RenewalAddItemToChartModel
    {
        [JsonProperty("ivr")]
        public bool IsIVR { get; set; }

        [JsonProperty("tkn")]
        public string Token { get; set; }

        [JsonProperty("htkn")]
        public string HashedToken { get; set; }

        [JsonProperty("lang")]
        public string Lang { get; set; }
    }
}
