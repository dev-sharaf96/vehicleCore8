using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    [JsonObject("ticketAttachedFileToDownloadModel")]
    public class TicketAttachedFileToDownloadModel
    {
        /// <summary>
        /// Extension
        /// </summary>
        [JsonProperty("extension")]
        public string Extension { get; set; }

        /// <summary>
        /// File
        /// </summary>
        [JsonProperty("file")]
        public string File { get; set; }
    }
}