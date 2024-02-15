using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Core.Domain.Dtos;

namespace Tameenk.Services.IdentityApi.Models
{
    /// <summary>
    /// change Password model
    /// </summary>
    [JsonObject("changePassword")]
    public class ChangePasswordModel : BaseViewModel
    {
        /// <summary>
        /// Email
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        [JsonProperty("password")]
        public string Password { get; set; }

        /// <summary>
        /// Confirm New Password
        /// </summary>
        [JsonProperty("confirmNewPassword")]
        public string ConfirmNewPassword { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; set; }

        /// <summary>
        /// User Id
        /// </summary>
        [JsonProperty("userId")]
        public string UserId { get; set; }

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
                
                if (string.IsNullOrEmpty(Password))
                {
                    ModelErrors.Add("Password مطلوب*");
                    return false;
                }

                if (string.IsNullOrEmpty(ConfirmNewPassword))
                {
                    ModelErrors.Add("Confirm New Password مطلوب*");
                    return false;
                }

                if (string.IsNullOrEmpty(Token))
                {
                    ModelErrors.Add("Token مطلوب*");
                    return false;
                }

                if (string.IsNullOrEmpty(UserId))
                {
                    ModelErrors.Add("User Id مطلوب*");
                    return false;
                }
                return true;
            }
        }
        public List<string> ModelErrors { get; set; }
       
    }
}