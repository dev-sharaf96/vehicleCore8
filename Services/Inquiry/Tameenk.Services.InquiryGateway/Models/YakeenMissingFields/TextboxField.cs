using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.InquiryGateway.Models.YakeenMissingFields
{
    public class TextboxField : YakeenMissingFieldBase
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        public TextboxField(string type = "text")
        {
            ControlType = "textbox";
            Type = type;

        }
    }
}