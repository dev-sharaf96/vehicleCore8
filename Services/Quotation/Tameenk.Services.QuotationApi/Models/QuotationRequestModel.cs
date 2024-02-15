using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Tameenk.Services.QuotationApi.Models
{
    /// <summary>
    /// Represent Quotation Request
    /// </summary>
    [JsonObject("quotationRequest")]
    public class QuotationRequestModel
    {
        /// <summary>
        /// id
        /// </summary>
        [JsonProperty("id")]
        public int ID { get; set; }

        /// <summary>
        /// External id
        /// </summary>
        [JsonProperty("externalId")]
        public string ExternalId { get; set; }

        /// <summary>
        /// Main Driver id
        /// </summary>
        [JsonProperty("mainDriverId")]
        public Guid MainDriverId { get; set; }

        /// <summary>
        /// City Code
        /// </summary>
        [JsonProperty("cityCode")]
        public long CityCode { get; set; }

        /// <summary>
        /// Rquest policy Effective Date
        /// </summary>
        [JsonProperty("requestPolicyEffectiveDate")]
        public DateTime? RequestPolicyEffectiveDate { get; set; }

        /// <summary>
        /// Vehicle id
        /// </summary>
        [JsonProperty("vehicleId")]
        public Guid VehicleId { get; set; }

        /// <summary>
        /// user id
        /// </summary>
        [JsonProperty("userId")]
        public string UserId { get; set; }

        /// <summary>
        /// najm Ncd reference
        /// </summary>
        [JsonProperty("najmNcdRefrence")]
        public string NajmNcdRefrence { get; set; }

        /// <summary>
        /// Najm Ncd Free years
        /// </summary>
        [JsonProperty("najmNcdFreeYears")]
        public int? NajmNcdFreeYears { get; set; }

        /// <summary>
        /// Created Date Time
        /// </summary>
        [JsonProperty("createdDateTime")]
        public DateTime CreatedDateTime { get; set; }


        /// <summary>
        /// Is Comprehensive Generated
        /// </summary>
        [JsonProperty("isComprehensiveGenerated")]
        public bool IsComprehensiveGenerated { get; set; }


        /// <summary>
        /// Is comprehensive Requested
        /// </summary>
        [JsonProperty("isComprehensiveRequested")]
        public bool IsComprehensiveRequested { get; set; }

        /// <summary>
        /// City
        /// </summary>
        [JsonProperty("city")]
        public CityModel City { get; set; }

        /// <summary>
        /// Driver
        /// </summary>
        [JsonProperty("driver")]
        public DriverModel Driver { get; set; }

        /// <summary>
        /// Vehicle
        /// </summary>
        [JsonProperty("vehicle")]
        public VehicleModel Vehicle { get; set; }

        /// <summary>
        /// Remaining Time To Expire In Seconds
        /// </summary>
        [JsonProperty("remainingTimeToExpireInSeconds")]
        public double RemainingTimeToExpireInSeconds { get; set; }

        [NotMapped]
        [JsonProperty("QuotationExceptionModel")]
        public QuotationExceptionModel QuotationExceptionModel { set; get; }
    }
}