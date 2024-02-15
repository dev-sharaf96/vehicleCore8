using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    public class AttachedFiles
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
        public byte[] File { get; set; }


        /// <summary>
        /// TicketTypeFileId
        /// </summary>
        [JsonProperty("ticketTypeFileNameId")]
        public int TicketTypeFileId { get; set; }
    }
}