using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    /// <summary>
    /// Map policy Status to model
    /// </summary>
    [JsonObject("policyStatus")]
    public class PolicyStatusModel
    {
        /// <summary>
        /// Id
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Key
        /// </summary>
        [JsonProperty("key")]
        public string Key { get; set; }


        /// <summary>
        /// name Ar
        /// </summary>
        [JsonProperty("nameAr")]
        public string NameAr { get; set; }


        /// <summary>
        /// name En
        /// </summary>
        [JsonProperty("nameEn")]
        public string NameEn { get; set; }
    }
}