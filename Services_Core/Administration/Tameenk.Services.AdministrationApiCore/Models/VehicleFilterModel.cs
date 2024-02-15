using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    /// <summary>
    /// Vehicle Filter model
    /// </summary>
    [JsonObject("vehicleFilter")]
    public class VehicleFilterModel
    {
        /// <summary>
        /// Chassis Number
        /// </summary>
        [JsonProperty("chassisNumber")]
        public string ChassisNumber { get; set; }


        /// <summary>
        /// Sequence Number
        /// </summary>
        [JsonProperty("sequenceNumber")]
        public string SequenceNumber { get; set; }

        /// <summary>
        /// Custom Card Number
        /// </summary>
        [JsonProperty("customCardNumber")]
        public string CustomCardNumber { get; set; }
        /// <summary>
        /// Vehicle Model Code
        /// </summary>
        [JsonProperty("vehicleModelCode")]
        public long? VehicleModelCode { get; set; }

      

        /// <summary>
        /// Vehicle Maker Code
        /// </summary>
        [JsonProperty("vehicleMakerCode")]
        public short? VehicleMakerCode { get; set; }


        /// <summary>
        /// Car Plate Number.
        /// </summary>
        [JsonProperty("carPlateNumber")]
        public Nullable<short> CarPlateNumber { get; set; }
    }
}