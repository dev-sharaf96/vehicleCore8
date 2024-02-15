using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Policies
{
    public class VehicleSuccessPoliciesForAddBenefitsListing
    {
        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

        [JsonProperty("policyNo")]
        public string PolicyNo { get; set; }

        [JsonProperty("insuredNIN")]
        public string InsuredNIN { get; set; }

        [JsonProperty("policyIssueDate")]
        public DateTime? PolicyIssueDate { get; set; }

        [JsonProperty("policyExpiryDate")]
        public DateTime? PolicyExpiryDate { get; set; }

        [JsonProperty("insuranceCompanyNameAr")]
        public string InsuranceCompanyNameAr { get; set; }

        [JsonProperty("insuranceCompanyNameEn")]
        public string InsuranceCompanyNameEn { get; set; }
    }
}
