using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Policies
{
    public class InsuranceProposalPoliciesFromDBModel
    {
        /// <summary>
        /// Reference Id
        /// </summary>
        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

        /// <summary>
        /// Policy No
        /// </summary>
        [JsonProperty("policyNo")]
        public string PolicyNo { get; set; }

        /// <summary>
        /// External Id
        /// </summary>
        [JsonProperty("externalId")]
        public string ExternalId { get; set; }

        /// <summary>
        /// National Id
        /// </summary>
        [JsonProperty("nationalId")]
        public string NationalId { get; set; }

        /// <summary>
        /// Sequence Number
        /// </summary>
        [JsonProperty("sequenceNumber")]
        public string SequenceNumber { get; set; }

        /// <summary>
        /// CustomCard Number
        /// </summary>
        [JsonProperty("customCardNumber")]
        public string CustomCardNumber { get; set; }
    }
}
