using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.PolicyApi.Models
{
    /// <summary>
    /// Class of najm statistics.
    /// </summary>
    [JsonObject("najmStatistics")]
    public class NajmStatisticsModel
    {
        /// <summary>
        /// Get or set the count of submited polices.
        /// </summary>
        [JsonProperty("submited")]
        public int Submited { get; set; }

        /// <summary>
        /// Get or set the count of rejected policies.
        /// </summary>
        [JsonProperty("rejected")]
        public int Rejected { get; set; }

        /// <summary>
        /// Get or set the count of pending policies.
        /// </summary>
        [JsonProperty("pending")]
        public int Pending { get; set; }
    }
}