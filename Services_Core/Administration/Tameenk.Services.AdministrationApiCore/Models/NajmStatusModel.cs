using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    /// <summary>
    /// Najm status model
    /// </summary>
    [JsonObject("najmStatus")]
    public class NajmStatusModel
    {
        /// <summary>
        /// Najm status id
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Najm Status code
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// Najm Status Name Ar
        /// </summary>
        [JsonProperty("nameAr")]
       public  string NameAr { get; set; }

        /// <summary>
        /// Najm Status Name En
        /// </summary>
        [JsonProperty("nameEn")]
        public string NameEn { get; set; }

    }
}