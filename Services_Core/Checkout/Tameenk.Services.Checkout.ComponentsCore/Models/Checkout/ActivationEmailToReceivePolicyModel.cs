using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Tameenk.Core.Domain.Dtos;

namespace Tameenk.Services.Checkout.Components
{
    /// <summary>
    /// ActivationEmailToReceivePolicyModel
    /// </summary>
    public class ActivationEmailToReceivePolicyModel
    {
        /// <summary>
        /// UserId
        /// </summary>
        [JsonProperty("userId")]
        public string UserId { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// RequestedDate
        /// </summary>
        [JsonProperty("requestedDate")]
        public DateTime RequestedDate { get; set; }

        /// <summary>
        /// ReferenceId
        /// </summary>
        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }
    }
}