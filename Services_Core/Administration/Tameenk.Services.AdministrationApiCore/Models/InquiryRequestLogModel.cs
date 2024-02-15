using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    /// <summary>
    /// Inquiry request model
    /// </summary>
    public class InquiryRequestLogModel
    {
        /// <summary>
        /// Id
        /// </summary>
        [JsonProperty("Id")]
        public int ID { get; set; }


        /// <summary>
        /// Create date
        /// </summary>
        [JsonProperty("createdDate")]
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// user id
        /// </summary>
        [JsonProperty("userId")]
        public Guid UserID { get; set; }

        /// <summary>
        /// user name
        /// </summary>
        [JsonProperty("userName")]
        public string UserName { get; set; }

        /// <summary>
        /// User IP
        /// </summary>
        [JsonProperty("userIP")]
        public string UserIP { get; set; }

        /// <summary>
        /// User Agent
        /// </summary>
        [JsonProperty("userAgent")]
        public string UserAgent { get; set; }

        /// <summary>
        /// Server IP
        /// </summary>
        [JsonProperty("serverIP")]
        public string ServerIP { get; set; }

        /// <summary>
        /// Channel
        /// </summary>
        [JsonProperty("channel")]
        public string Channel { get; set; }

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
        /// Methods
        /// </summary>
        [JsonProperty("methodName")]
        public string MethodName { get; set; }

        /// <summary>
        /// VehicleId
        /// </summary>
        [JsonProperty("vehicleId")]
        public string VehicleId { get; set; }

        /// <summary>
        /// Driver Nin
        /// </summary>
        [JsonProperty("nin")]
        public string NIN { get; set; }

        /// <summary>
        /// City Code
        /// </summary>
        [JsonProperty("cityCode")]
        public int? CityCode { get; set; }

        /// <summary>
        /// External Id
        /// </summary>
        [JsonProperty("externalId")]
        public string externalId { get; set; }

        /// <summary>
        /// Policy Effective Date
        /// </summary>
        [JsonProperty("policyEffectiveDate")]
        public DateTime? PolicyEffectiveDate { get; set; }

        /// <summary>
        /// Najm Ncd Refrence
        /// </summary>
        [JsonProperty("najmNcdRefrence")]
        public string NajmNcdRefrence { get; set; }

        /// <summary>
        /// Najm Ncd Free Years
        /// </summary>
        [JsonProperty("najmNcdFreeYears")]
        public int? NajmNcdFreeYears { get; set; }

        /// <summary>
        /// Request Id
        /// </summary>
        [JsonProperty("requestId")]
        public Guid? RequestId { get; set; }

        /// <summary>
        /// Service Request
        /// </summary>
        [JsonProperty("serviceRequest")]
        public string ServiceRequest { get; set; }
    }
}