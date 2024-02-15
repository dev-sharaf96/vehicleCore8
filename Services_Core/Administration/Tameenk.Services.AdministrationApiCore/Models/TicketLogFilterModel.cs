using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    public class TicketLogFilterModel
    {
        /// <summary>
        /// Id
        /// </summary>
        [JsonProperty("id")]
        public int? Id { get; set; }

        /// <summary>
        /// Method Name
        /// </summary>
        [JsonProperty("methodName")]
        public string MethodName { get; set; }

        /// <summary>
        /// Channel
        /// </summary>
        [JsonProperty("channel")]
        public int? ChannelId { get; set; }

        /// <summary>
        /// National Id
        /// </summary>
        [JsonProperty("nin")]
        public string NationalId { get; set; }

        /// <summary>
        /// Reference Id
        /// </summary>
        [JsonProperty("referenceNo")]
        public string ReferenceNo { get; set; }

        /// <summary>
        /// Start date
        /// </summary>
        [JsonProperty("fromDate")]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// End date
        /// </summary>
        [JsonProperty("toDate")]
        public DateTime? EndDate { get; set; }
    }
}