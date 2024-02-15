using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    /// <summary>
    /// Service request model
    /// </summary>
    public class ServiceRequestModel
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
        public Guid? UserID { get; set; }

        /// <summary>
        /// user name
        /// </summary>
        [JsonProperty("userName")]
        public string UserName { get; set; }


        /// <summary>
        /// Methods
        /// </summary>
        [JsonProperty("method")]
        public string Method { get; set; }

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
        /// Service Request
        /// </summary>
        [JsonProperty("serviceRequest")]
        public string ServiceRequest { get; set; }


        /// <summary>
        /// Service Response
        /// </summary>
        [JsonProperty("serviceResponse")]
        public string ServiceResponse { get; set; }


        /// <summary>
        /// Service Response Time In Seconds
        /// </summary>
        [JsonProperty("serviceResponseTimeInSeconds")]
        public double? ServiceResponseTimeInSeconds { get; set; }



        /// <summary>
        /// Service Error Code
        /// </summary>
        [JsonProperty("serviceErrorCode")]
        public string ServiceErrorCode { get; set; }


        /// <summary>
        /// Service Error Description
        /// </summary>
        [JsonProperty("serviceErrorDescription")]
        public string ServiceErrorDescription { get; set; }

        /// <summary>
        /// Reference Id
        /// </summary>
        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

        /// <summary>
        /// Insurance Type Code
        /// </summary>
        [JsonProperty("insuranceTypeCode")]
        public int InsuranceTypeCode { get; set; }

        /// <summary>
        /// Driver Nin
        /// </summary>
        [JsonProperty("driverNin")]
        public string DriverNin { get; set; }

        /// <summary>
        /// VehicleId
        /// </summary>
        [JsonProperty("vehicleId")]
        public string VehicleId { get; set; }
    }
}