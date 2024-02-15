using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Api.Core.Models;

namespace Tameenk.Services.Inquiry.Components
{
    public class DropdownField : YakeenMissingFieldBase
    {
        [JsonProperty("options")]
        public List<IdNamePairModel> Options { get; set; }

        public DropdownField()
        {
            ControlType = "dropdown";
        }
    }
}