using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.InquiryGateway.Models.YakeenMissingFields
{
    public class YakeenMissingFieldBase
    {
        [JsonProperty("value")]
        public object Value { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }


        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("required")]
        public bool Required { get; set; }


        [JsonProperty("order")]
        public int Order { get; set; }

        [JsonProperty("controlType")]
        public string ControlType { get; set; }
    }
}