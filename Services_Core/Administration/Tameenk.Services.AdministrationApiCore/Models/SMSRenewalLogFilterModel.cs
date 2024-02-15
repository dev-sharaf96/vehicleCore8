using Newtonsoft.Json;
using System;

namespace Tameenk.Services.AdministrationApi.Models
{
    /// <summary>
    /// SMS Renewal Log filter
    /// </summary>
    [JsonObject("smsRenewalLogFilter")]
    public class SMSRenewalLogFilterModel
    {
        /// <summary>
        /// Start date
        /// </summary>
        /// 
        [JsonProperty("startDate")]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// End date
        /// </summary>
        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }


        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("isExport")]
        public bool IsExport { get; set; }
    }
}