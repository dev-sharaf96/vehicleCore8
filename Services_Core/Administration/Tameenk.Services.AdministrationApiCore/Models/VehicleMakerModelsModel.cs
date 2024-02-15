using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    [JsonObject("vehicleMakerModels")]
    public class VehicleMakerModelsModel
    {
        /// <summary>
        /// Code
        /// </summary>
        [JsonProperty("code")]
        public int Code { get; set; }

        /// <summary>
        /// Maker Code
        /// </summary>
        [JsonProperty("makerCode")]
        public int MakerCode { get; set; }

        /// <summary>
        /// English Description
        /// </summary>
        [JsonProperty("englishDescription")]
        public string EnglishDescription { get; set; }

        /// <summary>
        /// Arabic Description
        /// </summary>
        [JsonProperty("arabicDescription")]
        public string ArabicDescription { get; set; }
    }
}