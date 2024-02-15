using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    /// <summary>
    /// product type
    /// </summary>
    [JsonObject("product-type")]
    public class ProductTypeModel
    {
        /// <summary>
        /// product type code
        /// </summary>
        [JsonProperty("id")]
        public short Code { get; set; }

        /// <summary>
        /// English description
        /// </summary>
        [JsonProperty("descEN")]
        public string EnglishDescription { get; set; }


        /// <summary>
        /// Arabic description
        /// </summary>
        [JsonProperty("descAR")]
        public string ArabicDescription { get; set; }
    }
}