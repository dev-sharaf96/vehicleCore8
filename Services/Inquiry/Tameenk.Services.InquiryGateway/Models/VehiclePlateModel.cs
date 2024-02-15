﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.InquiryGateway.Models
{
    /// <summary>
    /// Represent the Vehicle Plate model.
    /// </summary>
    [JsonObject("vehiclePlateModel")]
    public class VehiclePlateModel
    {
        /// <summary>
        /// Plate Color.
        /// </summary>
        [JsonProperty("plateColor")]
        public string PlateColor { get; set; }

        /// <summary>
        /// Car Plate Text1.
        /// </summary>
        [JsonProperty("carPlateText1")]
        public string CarPlateText1 { get; set; }

        /// <summary>
        /// Car Plate Text2.
        /// </summary>
        [JsonProperty("carPlateText2")]
        public string CarPlateText2 { get; set; }

        /// <summary>
        /// Car Plate Text3.
        /// </summary>
        [JsonProperty("carPlateText3")]
        public string CarPlateText3 { get; set; }

        /// <summary>
        /// Car Plate Number.
        /// </summary>
        [JsonProperty("carPlateNumber")]
        public Nullable<short> CarPlateNumber { get; set; }

        /// <summary>
        /// Car Plate Number Ar.
        /// </summary>
        [JsonProperty("carPlateNumberAr")]
        public string CarPlateNumberAr { get; set; }

        /// <summary>
        /// Car Plate Number En.
        /// </summary>
        [JsonProperty("carPlateNumberEn")]
        public string CarPlateNumberEn { get; set; }

        /// <summary>
        /// The Car Plate Text Ar.
        /// </summary>
        [JsonProperty("carPlateTextAr")]
        public string CarPlateTextAr { get; set; }

        /// <summary>
        /// The Car Plate Text En .
        /// </summary>
        [JsonProperty("carPlateTextEn")]
        public string CarPlateTextEn { get; set; }
    }
}