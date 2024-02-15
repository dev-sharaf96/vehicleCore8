using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.InquiryGateway.Models
{
    /// <summary>
    /// Saudi post api result model.
    /// </summary>
    [JsonObject("saudiPostResult")]
    public class SaudiPostApiResultModel
    {
        /// <summary>
        /// Total search results
        /// </summary>
        [JsonProperty("")]
        public string totalSearchResults { get; set; }
        /// <summary>
        /// Addresses
        /// </summary>
        [JsonProperty("sddresses")]
        public List<SaudiPostAddressModel> Addresses { get; set; }
        /// <summary>
        /// Post code
        /// </summary>
        [JsonProperty("postCode")]
        public object PostCode { get; set; }
        /// <summary>
        /// success
        /// </summary>
        [JsonProperty("success")]
        public bool success { get; set; }
        /// <summary>
        /// result
        /// </summary>
        [JsonProperty("result")]
        public object result { get; set; }
        /// <summary>
        /// status description
        /// </summary>
        [JsonProperty("statusDescription")]
        public string statusdescription { get; set; }
        /// <summary>
        /// full exception
        /// </summary>
        [JsonProperty("fullException")]
        public object fullexception { get; set; }
    }
}