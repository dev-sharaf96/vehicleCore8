using Newtonsoft.Json;
using System.Collections.Generic;
using Tameenk.Core.Domain.Dtos;

namespace Tameenk.Services.IdentityApi.Models
{
    /// <summary>
    /// VerifyCodeModel
    /// </summary>
    [JsonObject("verify")]
    public class VerifyCodeModel
    {
        /// <summary>
        /// UserId
        /// </summary>
        [JsonProperty("userId")]
        public string UserId { get; set; }

        /// <summary>
        /// Code
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// PhoneNumber
        /// </summary>
        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; } = "ar";

        [JsonProperty("channel")]
        public string Channel { get; set; } = "Portal";

        public bool IsValid
        {
            get
            {
                ModelErrors = new List<string>();
                if (string.IsNullOrEmpty(UserId))
                {
                    ModelErrors.Add("UserId مطلوب*");
                    return false;
                }
                if (string.IsNullOrEmpty(Code))
                {
                    ModelErrors.Add("Code مطلوب*");
                    return false;
                }
                if (string.IsNullOrEmpty(PhoneNumber))
                {
                    ModelErrors.Add("PhoneNumber مطلوب*");
                    return false;
                }

                return true;
            }

        }

        public List<string> ModelErrors { get; set; }

    }

    public class ResendVerifyCodeModel
    {
        /// <summary>
        /// UserId
        /// </summary>
        [JsonProperty("userId")]
        public string UserId { get; set; }

        /// <summary>
        /// PhoneNumber
        /// </summary>
        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; } = "ar";

        [JsonProperty("channel")]
        public string Channel { get; set; } = "Portal";


        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

        [JsonProperty("nin")]
        public string Nin { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        public bool IsValid
        {
            get
            {
                ModelErrors = new List<string>();
                if (string.IsNullOrEmpty(UserId))
                {
                    ModelErrors.Add("UserId مطلوب*");
                    return false;
                }
                if (string.IsNullOrEmpty(PhoneNumber))
                {
                    ModelErrors.Add("PhoneNumber مطلوب*");
                    return false;
                }

                return true;
            }

        }

        public List<string> ModelErrors { get; set; }

    }
}