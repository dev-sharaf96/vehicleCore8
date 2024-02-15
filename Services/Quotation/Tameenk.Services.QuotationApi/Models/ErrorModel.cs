using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.QuotationApi.Models
{
    [JsonObject("errorModel")]
    public class ErrorModel
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("field")]
        public string Field { get; set; }
    }
}