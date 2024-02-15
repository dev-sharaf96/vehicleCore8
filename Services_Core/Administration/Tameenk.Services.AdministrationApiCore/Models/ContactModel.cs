using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    [JsonObject("contact")]
    public class ContactModel
    {
        /// <summary>
        /// Id 
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// MobileNumber
        /// </summary>
        [JsonProperty("mobileNumber")]
        public string MobileNumber { get; set; }

        /// <summary>
        /// HomePhone
        /// </summary>
        [JsonProperty("homePhone")]
        public string HomePhone { get; set; }

        /// <summary>
        /// Fax
        /// </summary>
        [JsonProperty("fax")]
        public string Fax { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }
    }
}