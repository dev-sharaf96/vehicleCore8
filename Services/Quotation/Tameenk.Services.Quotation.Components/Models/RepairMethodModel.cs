using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.Quotation.Components
{
    [JsonObject("repairMethodModel")]
    public class RepairMethodModel
    {
        [JsonProperty("repairMethod")]
        public string RepairMethod { get; set; }

        [JsonProperty("isReadOnly")]
        public bool IsReadOnly { get; set; } = false;
    }
}