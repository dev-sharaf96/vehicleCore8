using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Policies
{
   public class NajmResponseTimeFilter
    {
        [JsonProperty("startDate")]
        public DateTime? StartDate { get; set; }

        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }

        [JsonProperty("referenceNo")]
        public string ReferenceNo { get; set; }

        [JsonProperty("policyNo")]
        public string PolicyNo { get; set; }
 
        [JsonProperty("companyId")]
        public int? CompanyId { get; set; }

        [JsonProperty("exports")]
        public bool Exports { get; set; }
    }
}
