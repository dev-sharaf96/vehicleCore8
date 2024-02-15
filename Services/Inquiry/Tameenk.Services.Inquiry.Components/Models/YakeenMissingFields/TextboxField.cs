using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.Inquiry.Components
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