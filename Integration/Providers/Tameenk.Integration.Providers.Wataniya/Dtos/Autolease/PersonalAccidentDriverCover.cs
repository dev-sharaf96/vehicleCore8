using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Providers.Wataniya.Dtos.Autolease
{
    class PersonalAccidentDriverCover
    {
        [JsonProperty("CoverageCode")]
        public string CoverageCode { get; set; }
        [JsonProperty("Limit")]
        public float Limit { get; set; }
        [JsonProperty("ActualPremium")]
        public float ActualPremium { get; set; }
    }
}
