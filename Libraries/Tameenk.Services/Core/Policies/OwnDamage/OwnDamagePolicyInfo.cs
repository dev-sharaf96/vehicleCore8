using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core.Policies
{
 public   class OwnDamagePolicyInfo
    {
        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

        [JsonProperty("policyNo")]
        public string PolicyNo { get; set; }

        [JsonProperty("insuranceCompanyName")]
        public string InsuranceCompanyName { get; set; }

        [JsonProperty("policyIssueDate")]
        public DateTime? PolicyIssueDate { get; set; }
        [JsonProperty("policyExpiryDate")]
        public DateTime? PolicyExpiryDate { get; set; }
    }
}
