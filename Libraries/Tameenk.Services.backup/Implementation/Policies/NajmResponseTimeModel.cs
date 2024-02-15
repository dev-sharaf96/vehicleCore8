using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Policies
{
   public class NajmResponseTimeModel
    {
        [JsonProperty("companyName")]
        public string CompanyName { get; set; }

        [JsonProperty("policyNo")]
        public string PolicyNo { get; set; }

        [JsonProperty("responseTime")]
        public double? ResponseTime { get; set; }

        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

        [JsonProperty("fullName")]
        public string FullName { get; set; }
        [JsonProperty("vehicleId")]
        public string VehicleId { get; set; }

        [JsonProperty("najmStatus")]
        public string NajmStatus { get; set; }
        [JsonProperty("nationalId")]
        public string NationalId { get; set; }
        [JsonProperty("nIN")]
        public string NIN { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("policyExpiryDate")]
        public DateTime? PolicyExpiryDate { get; set; }
        [JsonProperty("PolicyEffectiveDate")]
        public DateTime? PolicyEffectiveDate { get; set; }
        [JsonProperty("policyIssueDate")]
        public DateTime? PolicyIssueDate { get; set; }

    }
}
