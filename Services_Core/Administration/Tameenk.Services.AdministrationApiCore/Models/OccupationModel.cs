using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    public class OccupationModel
    {
        /// <summary>
        /// Id
        /// </summary>
        [JsonProperty("id")]
        public int ID { get; set; }

        /// <summary>
        /// Code
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }
        /// <summary>
        /// English Description
        /// </summary>
        [JsonProperty("englishDescription")]
        public string NameAr { get; set; }

        /// <summary>
        /// Arabic Description
        /// </summary>
        [JsonProperty("arabicDescription")]
        public string NameEn { get; set; }

        [JsonProperty("isCitizen")]
        public bool? IsCitizen { get; set; }

        [JsonProperty("isMale")]
        public bool? IsMale { get; set; }
    }
}