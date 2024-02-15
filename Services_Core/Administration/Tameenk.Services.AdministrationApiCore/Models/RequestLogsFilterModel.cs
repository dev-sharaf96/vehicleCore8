using Newtonsoft.Json;
using System;

namespace Tameenk.Services.AdministrationApi.Models
{

    /// <summary>
    /// Checkout Request filter
    /// </summary>
    [JsonObject("checkoutRequestFilter")]
    public class RequestLogsFilterModel
    {
        /// <summary>
        /// vehicle id represent ( sequence No / Custom No)
        /// </summary>
        [JsonProperty("vehicleId")]
        public string VehicleId { get; set; }

        /// <summary>
        /// National Id
        /// </summary>
        [JsonProperty("nin")]
        public string NIN { get; set; }

        /// <summary>
        /// Reference Id
        /// </summary>
        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

        /// <summary>
        /// External Id
        /// </summary>
        [JsonProperty("externalId")]
        public string ExternalId { get; set; }

        /// <summary>
        /// Start Date
        /// </summary>
        [JsonProperty("startDate")]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// End Date
        /// </summary>
        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// channel id
        /// </summary>
        [JsonProperty("channel")]
        public int? Channel { get; set; }

        /// <summary>
        /// Method Name
        /// </summary>
        [JsonProperty("methodName")]
        public string MethodName { get; set; }
    }
}