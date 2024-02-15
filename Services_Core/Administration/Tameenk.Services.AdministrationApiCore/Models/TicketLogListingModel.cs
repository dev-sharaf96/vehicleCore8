using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    public class TicketLogListingModel
    {
        /// <summary>
        /// Id
        /// </summary>
        [JsonProperty("id")]
        public int? Id { get; set; }

        /// <summary>
        /// createdDate
        /// </summary>
        [JsonProperty("createdDate")]
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// methodName
        /// </summary>
        [JsonProperty("methodName")]
        public string MethodName { get; set; }

        /// <summary>
        /// userAgent
        /// </summary>
        [JsonProperty("userAgent")]
        public string UserAgent { get; set; }

        /// <summary>
        /// channel
        /// </summary>
        [JsonProperty("channel")]
        public string Channel { get; set; }

        /// <summary>
        /// errorCode
        /// </summary>
        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }

        /// <summary>
        /// errorDescription
        /// </summary>
        [JsonProperty("errorDescription")]
        public string ErrorDescription { get; set; }

        /// <summary>
        /// driverNin
        /// </summary>
        [JsonProperty("driverNin")]
        public string DriverNin { get; set; }

        /// <summary>
        /// referenceId
        /// </summary>
        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }
    }
}