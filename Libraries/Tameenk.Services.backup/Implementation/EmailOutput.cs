using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation
{
    /// <summary>
    /// output Model
    /// </summary>
    [JsonObject("outputModel")]
    public class EmailOutput
    {
        [JsonProperty("errorCode")]
        public ErrorCodes ErrorCode
        {
            get;
            set;
        }
        /// <summary>
        /// error description
        /// </summary>
        [JsonProperty("errorDescription")]
        public string ErrorDescription { get; set; }

        /// <summary>
        /// error code
        /// </summary>
       
        public enum ErrorCodes
        {
            Success = 1,
            InvalidInput,
            NullResponse,
            UnspecifiedError,
            ServiceError,
            ServiceException,
            TokenIsEmpty,
            EmailNotSent
        }

        [JsonIgnore]
        public string LogErrorDescription { get; set; }
    }
}
