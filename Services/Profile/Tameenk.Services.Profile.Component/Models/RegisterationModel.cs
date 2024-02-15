using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Tameenk.Common.Utilities;

namespace Tameenk.Services.Profile.Component
{
    [JsonObject("register")]
    public class RegisterationModel 
    {
        /// <summary>
        /// Email
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// ConfirmEmail
        /// </summary>
        [JsonProperty("confirmemail")]
        public string ConfirmEmail { get; set; }

        /// <summary>
        /// Mobile
        /// </summary>
        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        [JsonProperty("password")]
        public string Password { get; set; }

        public string Language { get; set; } = "ar";
        public Channel Channel { get; set; } = Channel.Portal;

        public List<string> ModelErrors { get; set; }
        [JsonProperty("nationalId")]
        public string NationalId { get; set; }
        public int? OTP { get; set; }

        [JsonProperty("getBirthDate")]
        public bool GetBirthDate { get; set; } = false;

        [JsonProperty("birthMonth")]
        public int? BirthMonth { get; set; } = null;

        [JsonProperty("birthYear")]
        public int? BirthYear { get; set; } = null;

        [JsonProperty("fnar")]
        public string FullNameAr { get; set; }

        [JsonProperty("fnen")]
        public string FullNameEn { get; set; }

        [JsonProperty("hashed")]
        public string Hashed { get; set; }
    }
}