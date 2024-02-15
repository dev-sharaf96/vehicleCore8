using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Core.Domain.Dtos;

namespace Tameenk.Services.Profile.Component
{
    /// <summary>
    /// Confirm Password model
    /// </summary>
    [JsonObject("confirmResetPassword")]
    public class ConfirmResetPasswordModel : BaseViewModel
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("confirmNewPassword")]
        public string ConfirmNewPassword { get; set; }


    }
}