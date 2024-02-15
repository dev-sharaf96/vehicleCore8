using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    /// <summary>
    /// SMS Log Model
    /// </summary>
    public class SMSLogModel
    {
        /// <summary>
        /// Id
        /// </summary>
        [JsonProperty("Id")]
        public int ID { get; set; }

        /// <summary>
        /// Mobile Number
        /// </summary>
        [JsonProperty("mobileNumber")]
        public string MobileNumber { get; set; }

        /// <summary>
        /// SMS Message
        /// </summary>
        [JsonProperty("smsMessage")]
        public string SMSMessage { get; set; }

        /// <summary>
        /// Error Code
        /// </summary>
        [JsonProperty("errorCode")]
        public int? ErrorCode { get; set; }

        /// <summary>
        /// ErrorDescription
        /// </summary>
        [JsonProperty("errorDescription")]
        public string ErrorDescription { get; set; }

        /// <summary>
        /// Create date
        /// </summary>
        [JsonProperty("createdDate")]
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// method
        /// </summary>
        [JsonProperty("method")]
        public string Method { get; set; }
        public string UserIP { get; set; }

        public string ServerIP { get; set; }

        public string UserAgent { get; set; }

        public string SMSProvider { get; set; }

        public string ServiceURL { get; set; }

        public string ServiceRequest { get; set; }

        public string ServiceResponse { get; set; }

        public double? ServiceResponseTimeInSeconds { get; set; }
        public string ReferenceId { get; set; }
        public string Module { get; set; }
        public string Channel { get; set; }
    }
}