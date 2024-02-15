using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    [JsonObject("registerModel")]
    public class RegisterModel
    {
        [JsonProperty("CreatedBy")]
        public string CreatedByEmail { get; set; }

        [JsonProperty("users")]
        public List<RegisterUserModel> users { get; set; }
    }
}