using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    /// <summary>
    /// driver filter model
    /// </summary>
    [JsonObject("najmResponseFilter")]
    public class NajmResponseFilterModel
    {
        /// <summary>
        /// Policy Holder Nin
        /// </summary>
        [JsonProperty("policyHolderNin")]
        public string PolicyHolderNin { get; set; }
    }
}