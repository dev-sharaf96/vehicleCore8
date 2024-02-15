using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Integration.Dto.Providers;

namespace Tameenk.Integration.Providers.Tawuniya.Dtos
{
   public class AutoLeasingPolicyResponse
    {
        [JsonProperty("PolicyReferenceNo")]
        public string PolicyReferenceNo { get; set; }
        [JsonProperty("PolicyNumber")]
        public string PolicyNumber { get; set; }
        [JsonProperty("PolicyEffectiveDate")]
        public string PolicyEffectiveDate { get; set; }

        [JsonProperty("PolicyExpiryDate")]
        public string PolicyExpiryDate { get; set; }
        [JsonProperty("Status")]
        public string Status { get; set; }
        [JsonProperty("errors")]
        public List<Error> errors { get; set; }
    }
}
