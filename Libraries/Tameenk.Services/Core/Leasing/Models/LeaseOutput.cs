using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core.Leasing.Models
{
    public class LeaseOutput<TResult>
    {
        public enum ErrorCodes
        {
            Success = 1,
            EmptyInputParamter,
            NullResponse,
            ServiceError,
            ExceptionError,
            NotAuthorized
        }

        /// <summary>
        /// out put error Description
        /// </summary>
        [JsonProperty("errorDescription")]
        public string ErrorDescription { get; set; }

        /// <summary>
        /// out put error Code
        /// </summary>
        [JsonProperty("errorCode")]
        public ErrorCodes ErrorCode { get; set; }

        /// <summary>
        /// Result
        /// </summary>
        public TResult Result { get; set; }
    }
}
