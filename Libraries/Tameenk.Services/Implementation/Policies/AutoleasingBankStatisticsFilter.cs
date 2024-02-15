using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tameenk.Services.Implementation.Policies
{
    public class AutoleasingBankStatisticsFilter
    {
        /// <summary>
        /// Start Date
        /// </summary>
        [JsonProperty("startDate")]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Start Date
        /// </summary>
        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Insurance Company Id
        /// </summary>
        [JsonProperty("insuranceCompanyId")]
        public int? InsuranceCompanyId { get; set; }

        /// <summary>
        /// Insurance Company Id
        /// </summary>
        [JsonProperty("bankId")]
        public int? BankId { get; set; }
    }
}