using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    /// <summary>
    /// Policy Checkout Filter
    /// </summary>
    [JsonObject("policyCheckoutFilter")]
    public class PolicyCheckoutFilterModel
    {
        /// <summary>
        /// National id (NIN)
        /// </summary>
        [JsonProperty("nin")]
        public string NIN { get; set; }
    }
}