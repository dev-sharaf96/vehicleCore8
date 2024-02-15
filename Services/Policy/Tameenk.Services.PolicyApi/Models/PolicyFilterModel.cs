using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.PolicyApi.Models
{
    /// <summary>
    /// Map to Policy Filter
    /// </summary>
    [JsonObject("policyFilter")]
    public class PolicyFilterModel
    {
        /// <summary>
        /// Body type id
        /// </summary>
        [JsonProperty("bodyTypeId")]
        public int? BodyTypeId { get; set; }

        /// <summary>
        /// by Age from
        /// </summary>

        [JsonProperty("byAgeFrom")]
        public decimal? ByAgeFrom { get; set; }

        /// <summary>
        /// by Age to
        /// </summary>

        [JsonProperty("byAgeTo")]
        public decimal? ByAgeTo { get; set; }

        /// <summary>
        /// city id
        /// </summary>

        [JsonProperty("cityId")]
        public int? CityId { get; set; }


        /// <summary>
        /// issuance date from
        /// </summary>

        [JsonProperty("issuanceDateFrom")]
        public DateTime? IssuanceDateFrom { get; set; }

        /// <summary>
        /// issuance date to
        /// </summary>

        [JsonProperty("issuanceDateTo")]
        public DateTime? IssuanceDateTo { get; set; }

        /// <summary>
        /// policy No
        /// </summary>

        [JsonProperty("policyNumber")]
        public string PolicyNumber { get; set; }

        /// <summary>
        /// Product type id
        /// </summary>

        [JsonProperty("productTypeId")]
        public int? ProductTypeId { get; set; }

        /// <summary>
        /// vehicle maker id
        /// </summary>

        [JsonProperty("vehicleMakerId")]
        public int? VehicleMakerId { get; set; }

        /// <summary>
        /// vehicle maker model id
        /// </summary>

        [JsonProperty("vehicleMakerModelId")]
        public int? VehicleMakerModelId { get; set; }

        /// <summary>
        /// year
        /// </summary>

        [JsonProperty("year")]
        public int? Year { get; set; }
    }
}