using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.Implementation.Policies
{
    public class VehicleSuccessPoliciesFilterForAddBenefits
    {
        /// <summary>
        /// Reference No
        /// </summary>
        [JsonProperty("referenceNo")]
        public string ReferenceNo { get; set; }

        /// <summary>
        /// policy No
        /// </summary>
        [JsonProperty("policyNo")]
        public string PolicyNo { get; set; }

        [JsonProperty("insurredId")]
        public string InsurredId { get; set; }
    }
}