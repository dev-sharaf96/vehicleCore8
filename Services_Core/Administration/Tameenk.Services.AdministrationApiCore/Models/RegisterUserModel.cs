using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Core.Domain.Dtos;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Services.AdministrationApi.Models
{
    /// <summary>
    /// registeration model
    /// </summary>
    [JsonObject("register")]
    public class RegisterUserModel : BaseViewModel
    {
        /// <summary>
        /// FullName
        /// </summary>
        [JsonProperty("fullname")]
        public string FullName { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

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
        /// <summary>
        /// created By
        /// </summary>
        [JsonProperty("superAdminEmail")]
        public string adminEmail { get; set; }
        /// <summary>
        /// Bank Id
        /// </summary>
        [JsonProperty("bankId")]
        public int BankId { get; set; }

        [JsonProperty("isSuperAdmin")]
        public bool IsSuperAdmin { get; set; }



        public bool IsValid
        {
            get
            {
                ModelErrors = new List<string>();
                if (string.IsNullOrEmpty(Email))
                {
                    ModelErrors.Add("Email مطلوب*");
                    return false;
                }
                if (string.IsNullOrEmpty(Mobile))
                {
                    ModelErrors.Add("Mobile مطلوب*");
                    return false;
                }
                if (string.IsNullOrEmpty(Password))
                {
                    ModelErrors.Add("Password مطلوب*");
                    return false;
                }
                return true;
            }
        }
        public List<string> ModelErrors { get; set; }

    }
}