using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    [JsonObject("city")]
    public class CityModel
    {
        /// <summary>
        /// City code.
        /// </summary>
        [JsonProperty("code")]
        public long Code { get; set; }
        /// <summary>
        /// English Description.
        /// </summary>
        [JsonProperty("englishDescription")]
        public string EnglishDescription { get; set; }
        /// <summary>
        /// Arabic Description.
        /// </summary>
        [JsonProperty("arabicDescription")]
        public string ArabicDescription { get; set; }
    }
}