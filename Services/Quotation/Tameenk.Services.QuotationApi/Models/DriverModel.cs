using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.QuotationApi.Models
{
    /// <summary>
    /// Represent driver
    /// </summary>
    [JsonObject("driver")]
    public class DriverModel
    {
        /// <summary>
        /// Driver Full English Name
        /// </summary>
        [JsonProperty("fullEnglishName")]
        public string FullEnglishName { get; set; }

        /// <summary>
        /// Driver Full Arabic Name
        /// </summary>
        [JsonProperty("fullArabicName")]
        public string FullArabicName { get; set; }


        /// <summary>
        /// en First Name
        /// </summary>
        [JsonProperty("enFirstName")]
        public string EnglishFirstName { get; set; }

        /// <summary>
        /// en Last Name
        /// </summary>
        [JsonProperty("enLastName")]
        public string EnglishLastName { get; set; }

        /// <summary>
        /// en Second Name
        /// </summary>
        [JsonProperty("enSecondName")]
        public string EnglishSecondName { get; set; }

        /// <summary>
        /// en Third Name
        /// </summary>
        [JsonProperty("enThirdName")]
        public string EnglishThirdName { get; set; }


        /// <summary>
        /// Driver First Name
        /// </summary>
        [JsonProperty("firstName")]
        public string FirstName { get; set; }


        /// <summary>
        /// Driver last Name
        /// </summary>
        [JsonProperty("lastName")]
        public string LastName { get; set; }

        /// <summary>
        /// Driver Second Name
        /// </summary>
        [JsonProperty("secondName")]
        public string SecondName { get; set; }


        /// <summary>
        /// Driver third Name
        /// </summary>
        [JsonProperty("thirdName")]
        public string ThirdName { get; set; }
    }
}