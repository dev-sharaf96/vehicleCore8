using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Providers.Wataniya.Dtos.Autolease
{
    class GeographicalAreaExtension
    {
        [JsonProperty("CoverageCode")]
        public string CoverageCode { get; set; }
        [JsonProperty("Deductible")]
        public int Deductible { get; set; }
        [JsonProperty("CoveredCountry")]
        public int CoveredCountry { get; set; }
        [JsonProperty("From")]
        public DateTime From { get; set; }

        [JsonProperty("To")]
        public DateTime To { get; set; }
        [JsonProperty("ActualPremium")]
        public int ActualPremium { get; set; }
    }
}
