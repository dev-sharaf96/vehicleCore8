using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services
{
    public class CorporateAccountModel
    {
        /// <summary>
        /// user id
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// nameEn
        /// </summary>
        [JsonProperty("nameEn")]
        public string NameEn { get; set; }

        /// <summary>
        /// nameAr
        /// </summary>
        [JsonProperty("nameAr")]
        public string NameAr { get; set; }

        /// <summary>
        /// Balance
        /// </summary>
        [JsonProperty("balance")]
        public decimal? Balance { get; set; }

        /// <summary>
        /// account isLock
        /// </summary>
        [JsonProperty("isActive")]
        public bool IsActive { get; set; }

        /// <summary>
        /// lang
        /// </summary>
        [JsonProperty("lang")]
        public string lang { get; set; } = "en";
    }
}
