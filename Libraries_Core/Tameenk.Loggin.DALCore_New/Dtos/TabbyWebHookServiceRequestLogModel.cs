using Newtonsoft.Json;

namespace Tameenk.Loggin.DAL
{
    public class TabbyWebHookServiceRequestLogModel
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
        /// Reference Id
        /// </summary>
        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

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

        /// <summary>
        /// ServerIP
        /// </summary>
        [JsonProperty("serverIP")]
        public string ServerIP { get; set; }

        /// <summary>
        /// Channel
        /// </summary>
        [JsonProperty("channel")]
        public string Channel { get; set; }
    }
}
