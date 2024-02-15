using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    public class TicketHistoryModel
    {
        /// <summary>
        /// Id
        /// </summary>
        [JsonProperty("id")]
        public int? Id { get; set; }

        /// <summary>
        /// Ticket Id
        /// </summary>
        [JsonProperty("ticketId")]
        public int? TicketId { get; set; }

        /// <summary>
        /// Status Id
        /// </summary>
        [JsonProperty("statusId")]
        public int? StatusId { get; set; }

        /// <summary>
        /// status
        /// </summary>
        [JsonProperty("statusNameAr")]
        public string StatusNameAr { get; set; }
        [JsonProperty("statusNameEn")]
        public string StatusNameEn { get; set; }

        /// <summary>
        /// Admin Reply
        /// </summary>
        [JsonProperty("adminReply")]
        public string AdminReply { get; set; }

        /// <summary>
        /// Replied By
        /// </summary>
        [JsonProperty("repliedBy")]
        public string RepliedBy { get; set; }

        /// <summary>
        /// Created Date
        /// </summary>
        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; internal set; }
        [JsonProperty("userId")]
        public string UserId { get; internal set; }
    }
}