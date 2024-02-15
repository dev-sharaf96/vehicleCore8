using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tameenk.Services.Implementation.Policies
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

    }
}