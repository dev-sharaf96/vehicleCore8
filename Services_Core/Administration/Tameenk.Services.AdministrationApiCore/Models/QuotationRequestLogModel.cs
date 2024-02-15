using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    /// <summary>
    /// Quotation request model
    /// </summary>
    public class QuotationRequestLogModel
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
        public string UserID { get; set; }

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
        /// Insurance Type Code
        /// </summary>
        [JsonProperty("insuranceTypeCode")]
        public int? InsuranceTypeCode { get; set; }


        /// <summary>
        /// Company Id
        /// </summary>
        [JsonProperty("companyID")]
        public int? CompanyID { get; set; }

        /// <summary>
        /// Company Name
        /// </summary>
        [JsonProperty("companyName")]
        public string CompanyName { get; set; }

        /// <summary>
        /// Reference Id
        /// </summary>
        [JsonProperty("referenceId")]
        public string RefrenceId { get; set; }

        /// <summary>
        /// External Id
        /// </summary>
        [JsonProperty("externalId")]
        public string ExtrnlId { get; set; }
    }
}