using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.PolicyApi.Models
{
    /// <summary>
    /// Policy update request payment.
    /// </summary>
    [JsonObject("policyUpdatePayment")]
    public class PolicyUpdatePaymentModel
    {

        /// <summary>
        /// The identifier.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }
        /// <summary>
        /// Policy update request id
        /// </summary>
        [JsonProperty("policyUpdateRequestId")]
        public int PolicyUpdateRequestId { get; set; }

        /// <summary>
        /// Policy update amount
        /// </summary>
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Payment Description
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Created by
        /// </summary>
        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Creation date
        /// </summary>
        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }
    }
}