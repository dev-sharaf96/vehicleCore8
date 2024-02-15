using Newtonsoft.Json;
using System.Collections.Generic;
using Tameenk.Core.Domain.Dtos;

namespace Tameenk.Services.IdentityApi.Models
{
    /// <summary>
    /// VerifyCorporateCodeModel
    /// </summary>
    [JsonObject("verifyCorporateCodeModel")]
    public class VerifyCorporateCodeModel : BaseViewModel
    {
        /// <summary>
        /// Email
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// OTP
        /// </summary>
        [JsonProperty("otp")]
        public string OTP { get; set; }
    }
}