using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Core.Domain.Dtos;

namespace Tameenk.Services.Profile.Component
{
    /// <summary>
    /// forget Password model
    /// </summary>
    [JsonObject("forgetPassword")]
    public class ForgetPasswordModel : BaseViewModel
    {
        /// <summary>
        /// Email
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }


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

                return true;
            }
        }
        public List<string> ModelErrors { get; set; }
       
    }
}