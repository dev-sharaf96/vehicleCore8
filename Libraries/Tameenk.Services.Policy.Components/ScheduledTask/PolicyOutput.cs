using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.Policy.Components
{
    /// <summary>
    /// policy Model
    /// </summary>
    [JsonObject("PolicyOutput")]
    public class PolicyOutput
    {
        /// <summary>
        /// Policy Status model
        /// </summary>
        [JsonProperty("ErrorDescription")]
        public string ErrorDescription { get; set; }

        [JsonProperty("ErrorCode")]
        public int ErrorCode { get; set; }
        public string DriverNin { get; set; }
        public string VehicleId { get; set; }

    }
}