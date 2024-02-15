using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Policies
{
    public class RenewalDataModel
    {
        [JsonProperty("companyName")]
        public string CompanyName { get; set; }

        [JsonProperty("productType")]
        public string ProductType { get; set; }

        [JsonProperty("expiryDate")]
        public DateTime? ExpiryDate { get; set; }

        [JsonProperty("renewalDate")]
        public DateTime? RenewalDate { get; set; }

        [JsonProperty("lastAmount")]
        public decimal? LastAmount { get; set; }

        [JsonProperty("currentAmount")]
        public decimal? CurrentAmount { get; set; }

        [JsonProperty("sequenceNumber")]
        public string SequenceNumber { get; set; }

        [JsonProperty("carOwnerNIN")]
        public string CarOwnerNIN { get; set; }

        [JsonProperty("OldCompanyName")]
        public string OldCompanyName { get; set; }

        [JsonProperty("OldProductType")]
        public string OldProductType { get; set; }

        [JsonProperty("oldReferenceId")]
        public string OldReferenceId { get; set; }

        [JsonProperty("oldPolicyno")]
        public string OldPolicyNo { get; set; }

        [JsonProperty("newReferenceId")]
        public string NewReferenceId { get; set; }

        [JsonProperty("newPolicyno")]
        public string NewPolicyNo { get; set; }
    }
}
