using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    /// <summary>
    /// driver filter model
    /// </summary>
    [JsonObject("driverFilter")]
    public class DriverFilterModel
    {
        /// <summary>
        /// National ID
        /// </summary>
        [JsonProperty("nationalId")]
        public string NIN { get; set; }

        /// <summary>
        /// FirstName
        /// </summary>
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        /// <summary>
        /// Last Name
        /// </summary>
        [JsonProperty("lastName")]
        public string LastName { get; set; }


        /// <summary>
        /// Email
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// IBAN
        /// </summary>
        [JsonProperty("iban")]
        public string IBAN { get; set; }

        /// <summary>
        /// Phone
        /// </summary>
        [JsonProperty("phone")]
        public string Phone { get; set; }


    }
}