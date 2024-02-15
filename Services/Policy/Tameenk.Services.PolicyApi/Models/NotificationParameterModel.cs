using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.PolicyApi.Models
{
    /// <summary>
    /// The notification parameter
    /// </summary>
    [JsonObject("notificationParameter")]
    public class NotificationParameterModel
    {
        /// <summary>
        /// The identifier.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// The notification parameter name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The notification parameter value.
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}