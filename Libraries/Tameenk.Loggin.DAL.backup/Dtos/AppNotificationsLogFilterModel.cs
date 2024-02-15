using Newtonsoft.Json;
using System;

namespace Tameenk.Loggin.DAL
{
    //[JsonObject("smsLogFilter")]
    public class AppNotificationsLogFilterModel
    {
        /// <summary>
        /// Mobile Number
        /// </summary>
        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

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
        
        [JsonProperty("channel")]
        public int? Channel { get; set; }

        [JsonProperty("export")]
        public bool Export { get; set; }
    }
}