using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.Implementation.Policies
{
    /// <summary>
    /// Check Discount Output
    /// </summary>
    [JsonObject("CheckDiscountOutput")]
    public class CheckDiscountOutput
    {
        public enum ErrorCodes
        {
            Success = 1,
            EmptyInputParamter = 2,
            ServiceDown = 3,
            InvalidCaptcha = 4,
            ServiceException = 5,
            EmptyReturnObject = 6
        }

        /// <summary>
        /// Error Description
        /// </summary>
        [JsonProperty("ErrorDescription")]
        public string ErrorDescription { get; set; }

        /// <summary>
        /// Error Code
        /// </summary>
        [JsonProperty("ErrorCode")]
        public ErrorCodes ErrorCode { get; set; }

    }
}