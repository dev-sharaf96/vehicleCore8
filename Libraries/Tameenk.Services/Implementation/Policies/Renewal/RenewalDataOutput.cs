using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Policies
{
    public class RenewalDataOutput
    {
        [JsonProperty("renewalStatistics")]
        public List<RenewalStatisticsModel> RenewalStatistics { get; set; }

        [JsonProperty("renewalData")]
        public List<RenewalDataModel> RenewalData { get; set; }

        [JsonProperty("totalPoliciesCount")]
        public int TotalPoliciesCount { get; set; }

        [JsonProperty("renewalDataCount")]
        public int RenewalDataCount { get; set; }

        [JsonProperty("exportBase64String")]
        public string ExportBase64String { get; set; }
    }
}
