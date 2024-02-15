using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Integration.Dto.Quotation
{
    /// <summary>
    /// Represent City
    /// </summary>
    [JsonObject("city")]
    public class CityModel
    {
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