using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    /// <summary>
    /// SMS Log Model
    /// </summary>
    public class OtpOutputModel
    {
       
        /// <summary>
        /// Error Code
        /// </summary>
        [JsonProperty("errorCode")]
        public int? ErrorCode { get; set; }

        /// <summary>
        /// ErrorDescription
        /// </summary>
        [JsonProperty("errorDescription")]
        public string ErrorDescription { get; set; }

    }
}