using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core.Policies
{
   public class policyDetailForOD
    {
        [JsonProperty("externalId")]
        public string ExternalId { get; set; }

        [JsonProperty("policyNo")]
        public string PolicyNo { get; set; }

        [JsonProperty("policyExpiryDate")]
        public DateTime? PolicyExpiryDate { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }  

        [JsonProperty("selectedLanguage")]
        public int? SelectedLanguage { get; set; }
    }
}
