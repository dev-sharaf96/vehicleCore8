using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    /// <summary>
    /// Checkouts Filter Model
    /// </summary>
    [JsonObject("checkoutsFilter")]
    public class CheckoutsFilterModel
    {
        /// <summary>
        /// NIN
        /// </summary>
        [JsonProperty("NIN")]
        public string NIN { get; set; }

        /// <summary>
        /// Reference Id
        /// </summary>
        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

        /// <summary>
        /// Sequence Number
        /// </summary>
        [JsonProperty("sequenceNumber")]
        public string SequenceNumber { get; set; }
        [JsonProperty("merchantId")]
        public string MerchantId { get; set; }
    }
}