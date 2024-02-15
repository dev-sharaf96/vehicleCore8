using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Policies
{
    public class RenewalStatisticsModel
    {
        [JsonProperty("companyId")]
        public int CompanyId { get; set; }

        [JsonProperty("companyKey")]
        public string CompanyKey { get; set; }

        [JsonProperty("companyCount")]
        public int CompanyCount { get; set; }

        [JsonProperty("renewalCompanyCount")]
        public int RenewalCompanyCount { get; set; }

        /// <summary>
        /// will not mapped for angular
        /// </summary>
        public string SequenceNumber { get; set; }
    }
}
