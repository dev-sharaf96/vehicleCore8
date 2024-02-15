using Newtonsoft.Json;
using System;

namespace Tameenk.Services.Core.Policies
{
    public class DriverswithPolicyDetails
    {
        [JsonProperty("referenceId")]
        public  string? ReferenceId { get; set; }

        [JsonProperty("roNum")]
        public long RoNum { get; set; }

        [JsonProperty("isCorporateUser")]
        public int IsCorporateUser { get; set; }

        [JsonProperty("isCompanyUser")]
        public int IsCompanyUser { get; set; }

        [JsonProperty("loggedInEmail")]
        public  string? LoggedInEmail { get; set; }

        [JsonProperty("vehicleId")]
        public  string? VehicleId { get; set; }

        [JsonProperty("insuredNin")]
        public  string? DriverNin { get; set; }

        [JsonProperty("iBAN")]
        public  string? IBAN { get; set; }

        [JsonProperty("email")]
        public  string? Email { get; set; }

        [JsonProperty("phone")]
        public  string? Phone { get; set; }

        [JsonProperty("fullName")]
        public  string? FullName { get; set; }

        [JsonProperty("nationalId")]
        public  string? InsuredId { get; set; }

        [JsonProperty("channel")]
        public  string? Channel { get; set; }

        [JsonProperty("policyIssueDate")]
        public DateTime? PolicyIssueDate { get; set; }

        [JsonProperty("carOwnerName")]
        public  string? CarOwnerName { get; set; }

        [JsonProperty("carOwnerNIN")]
        public  string? CarOwnerNIN { get; set; }

        [JsonProperty("ownerTransfer")]
        public bool OwnerTransfer { get; set; }
    }
}
