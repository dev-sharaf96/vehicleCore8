using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.Capcha.API
{
    public class Output<TResult>
    {
        public enum ErrorCodes
        {
            Success = 1,
            Fail = 2,
        }
        public ErrorCodes ErrorCode { get; set; }
        [JsonProperty("ErrorDescription")]
        public string ErrorMsg { get; set; }
        public TResult Result { get; set; }
    }

}