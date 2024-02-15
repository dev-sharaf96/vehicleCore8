using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.Capcha.API
{
    public class ValidateCaptchaResponse
    {
        [JsonProperty("data")]
        public bool Data { get; set; }
        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }
        [JsonProperty("errors")]
        public string Errors { get; set; }
    }
}