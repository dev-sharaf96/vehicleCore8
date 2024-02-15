using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    /// <summary>
    /// Channel
    /// </summary>
    public class ChannelModel
    {
        /// <summary>
        /// The Channel name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }



        /// <summary>
        /// The Channel code.
        /// </summary>
        [JsonProperty("id")]
        public int Code { get; set; }


    }
}