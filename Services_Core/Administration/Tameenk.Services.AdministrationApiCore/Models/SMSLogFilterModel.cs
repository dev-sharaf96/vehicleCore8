using Newtonsoft.Json;
using System;

namespace Tameenk.Services.AdministrationApi.Models
{

    /// <summary>
    /// SMS Log filter
    /// </summary>
    [JsonObject("smsLogFilter")]
    public class SMSLogFilterModel
    {
        /// <summary>
        /// Mobile Number
        /// </summary>
        [JsonProperty("mobileNumber")]
        public string MobileNumber { get; set; }

        /// <summary>
        /// Status Code
        /// </summary>
        [JsonProperty("statusCode")]
        public int? StatusCode { get; set; }

        /// <summary>
        /// Start date
        /// </summary>
        [JsonProperty("startDate")]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// End date
        /// </summary>
        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// method
        /// </summary>
        [JsonProperty("method")]
        public string Method { get; set; }
        [JsonProperty("channel")]
        public int? Channel { get; set; }
        [JsonProperty("smsprovider")]
        public int? SMSProvider { get; set; }
    }
}