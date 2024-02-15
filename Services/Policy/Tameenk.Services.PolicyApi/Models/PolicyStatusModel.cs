using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.PolicyApi.Models
{
    /// <summary>
    /// policy status model
    /// </summary>
    [JsonObject("policyStatus")]
    public class PolicyStatusModel
    {
        /// <summary>
        /// Policy status key
        /// </summary>
        [JsonProperty("key")]
        public string Key { get; set; }

        /// <summary>
        /// Policy Status Name Ar
        /// </summary>
        [JsonProperty("nameAr")]
        public string NameAr { get; set; }

        /// <summary>
        /// Policy Status Name En
        /// </summary>
        [JsonProperty("nameEn")]
        public string NameEn { get; set; }


    }
}