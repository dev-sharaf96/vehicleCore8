using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.PolicyApi.Models
{
    /// <summary>
    /// Product Type Model
    /// </summary>
    [JsonObject("productType")]
    public class ProductTypeModel
    {
        /// <summary>
        /// Code
        /// </summary>
        public short Code { get; set; }

        /// <summary>
        /// English description
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