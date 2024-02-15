using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core.Policies.Renewal
{
   public class RenewalPolicyDetails
    {
        [JsonProperty("policyNo")]
        public string PolicyNo { get; set; }

        [JsonProperty("policyEffectiveDate")]
        public DateTime? PolicyEffectiveDate { get; set; }

        [JsonProperty("policyExpiryDate")]
        public DateTime? PolicyExpiryDate { get; set; }

        [JsonProperty("fullName")]
        public string FullName { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("vehicleMaker")]
        public string VehicleMaker { get; set; }

        [JsonProperty("vehicleModel")]
        public string VehicleModel { get; set; }

        [JsonProperty("externalId")]
        public string ExternalId { get; set; }

        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }
        
        [JsonProperty("plate")]
        public short? Plate { get; set; }

        [JsonProperty("sequenceNumber")]
        public string SequenceNumber { get; set; }

        [JsonProperty("customCardNumber")]
        public string CustomCardNumber { get; set; }

        [JsonProperty("nIN")]
        public string NIN { get; set; }


    }
}
