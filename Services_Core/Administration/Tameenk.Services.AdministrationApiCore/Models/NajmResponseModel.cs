using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    /// <summary>
    /// Najm Response model to represent Najm Response Entity
    /// </summary>
    [JsonObject("najmResponse")]
    public class NajmResponseModel
    {
        /// <summary>
        /// Id
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Is Vehicle Registered
        /// </summary>
        [JsonProperty("isVehicleRegistered")]
        public int? IsVehicleRegistered { get; set; }

        /// <summary>
        /// Policy Holder Nin
        /// </summary>
        [JsonProperty("policyHolderNin")]
        public string PolicyHolderNin { get; set; }

        /// <summary>
        /// Vehicle Id
        /// </summary>
        [JsonProperty("vehicleId")]
        public string VehicleId { get; set; }

        /// <summary>
        /// NCD Reference
        /// </summary>
        [JsonProperty("nCDReference")]
        public string NCDReference { get; set; }

        /// <summary>
        /// NCD Free Years
        /// </summary>
        [JsonProperty("nCDFreeYears")]
        public int? NCDFreeYears { get; set; }

        /// <summary>
        /// Created At
        /// </summary>
        [JsonProperty("createdAt")]
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Is Deleted
        /// </summary>
        [JsonProperty("isDeleted")]
        public bool? IsDeleted { get; set; }
    }
}