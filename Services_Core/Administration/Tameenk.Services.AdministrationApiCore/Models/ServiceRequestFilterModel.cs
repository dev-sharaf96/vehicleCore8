using Newtonsoft.Json;
using System;

namespace Tameenk.Services.AdministrationApi.Models
{

    /// <summary>
    /// Service Request filter
    /// </summary>
    [JsonObject("serviceRequestFilter")]
    public class ServiceRequestFilterModel
    {
        /// <summary>
        /// vehicle id represent ( sequence No / Custom No)
        /// </summary>
        [JsonProperty("vehicleId")]
        public string VehicleId { get; set; }


        /// <summary>
        /// National Id
        /// </summary>
        [JsonProperty("nationalId")]
        public string NationalId { get; set; }

        /// <summary>
        /// Reference Id
        /// </summary>
        [JsonProperty("referenceNo")]
        public string ReferenceNo { get; set; }


        /// <summary>
        /// Method
        /// </summary>
        [JsonProperty("method")]
        public string Method { get; set; }

        /// <summary>
        /// Status Code
        /// </summary>
        [JsonProperty("statusCode")]
        public int? StatusCode { get; set; }

        /// <summary>
        /// insurance Company Id
        /// </summary>
        [JsonProperty("insuranceCompanyId")]
        public int? InsuranceCompanyId { get; set; }

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

        [JsonProperty("policyNo")]
        public string PolicyNo { get; set; }
        [JsonProperty("insuranceTypeId")]
        public string InsuranceTypeId { get; set; }
    }
}