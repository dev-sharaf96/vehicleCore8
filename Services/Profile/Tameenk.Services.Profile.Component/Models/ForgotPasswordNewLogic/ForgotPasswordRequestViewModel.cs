using Newtonsoft.Json;
using Tameenk.Core.Domain.Dtos;

namespace Tameenk.Services.Profile.Component
{
    public class ForgotPasswordRequestViewModel : BaseViewModel
    {
        #region For Begin Forgot Method

        [JsonProperty("mobileOrEmail")]
        public string MobileOrEmail { get; set; }

        [JsonProperty("isResetByEmailOrMobile")]
        public bool IsResetByEmailOrMobile { get; set; }

        [JsonProperty("nationalId")]
        public string NationalId { get; set; }

        [JsonProperty("isResetByNationalId")]
        public bool IsResetByNationalId { get; set; }

        [JsonProperty("captchaInput")]
        public string CaptchaInput { get; set; }

        [JsonProperty("captchaToken")]
        public string CaptchaToken { get; set; }

        #endregion

        #region For End Forgot Method

        [JsonProperty("isResetByEmail")]
        public bool IsResetByEmail { get; set; }

        [JsonProperty("isResetByMobile")]
        public bool IsResetByMobile { get; set; }

        [JsonProperty("uid")]
        public string UserId { get; set; }

        #endregion

        [JsonProperty("hashed")]
        public string Hashed { get; set; }
    }
}
