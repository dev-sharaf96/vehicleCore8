using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.VehicleInsurance;

namespace Tameenk.Services.Implementation.Policies
{
    /// <summary>
    /// policy Model
    /// </summary>
    [JsonObject("policy")]
    public class PoliciesDuplicationModel
    {

        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

        [JsonProperty("isCorporateUser")]
        public int IsCorporateUser { get; set; }

        [JsonProperty("isCompanyUser")]
        public int IsCompanyUser { get; set; }

        [JsonProperty("loggedInEmail")]
        public string LoggedInEmail { get; set; }

        [JsonProperty("vehicleId")]
        public string VehicleId { get; set; }

        [JsonProperty("driverNin")]
        public string DriverNin { get; set; }

        [JsonProperty("insuredNin")]
        public string InsuredNin { get; set; }

        [JsonProperty("iBAN")]
        public string IBAN { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("fullNmae")]
        public string FullName { get; set; }

        [JsonProperty("nationalId")]
        public string InsuredId { get; set; }

        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("policyIssueDate")]
        public DateTime? PolicyIssueDate { get; set; }

        [JsonProperty("carOwnerName")]
        public string CarOwnerName { get; set; }

        [JsonProperty("carOwnerNIN")]
        public string CarOwnerNIN { get; set; }

        [JsonProperty("ownerTransfer")]
        public bool OwnerTransfer { get; set; }
    }
}
