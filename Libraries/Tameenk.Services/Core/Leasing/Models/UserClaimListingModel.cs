using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core
{
    public class UserClaimListingModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

        [JsonProperty("policyNo")]
        public string PolicyNo { get; set; }

        [JsonProperty("externalId")]
        public string ExternalId { get; set; }

        [JsonProperty("driverLicenseExpiryDate")]
        public string DriverLicenseExpiryDate { get; set; }

        [JsonProperty("driverLicenseTypeCode")]
        public int? DriverLicenseTypeCode { get; set; }

        [JsonProperty("iban")]
        public string Iban { get; set; }

        [JsonProperty("claimStatusId")]
        public int ClaimStatusId { get; set; }

        [JsonProperty("claimStatusName")]
        public string ClaimStatusName { get; set; }

        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty("history")]
        public List<UserClaimHistoryModel> History { get; set; }
    }
}