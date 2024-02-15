using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Dtos;

namespace Tameenk.Services.Profile.Component
{
    public class VerifyForgotPasswordOTPRequestModel : BaseViewModel
    {
        [JsonProperty("uid")]
        public string UserId { get; set; }

        [JsonProperty("otp")]
        public int? Otp { get; set; }

        [JsonProperty("newPassword")]
        public string NewPassword { get; set; }

        [JsonProperty("confirmNewPassword")]
        public string ConfirmNewPassword { get; set; }
    }
}
