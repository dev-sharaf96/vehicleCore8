using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.IdentityApi.Output
{
    /// <summary>
    /// Created Ticket Output
    /// </summary>
    [JsonObject("ProfileNotificationOutput")]
    public class ProfileNotificationOutput
    {
        /// <summary>
        /// Description
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Type Id
        /// </summary>
        [JsonProperty("typeId")]
        public int? TypeId { get; set; }

        /// <summary>
        /// Ticket Status Id
        /// </summary>
        [JsonProperty("ticketStatusId")]
        public int? TicketStatusId { get; set; }

        /// <summary>
        /// Module Id
        /// </summary>
        [JsonProperty("moduleId")]
        public int? ModuleId { get; set; }

        /// <summary>
        /// Created Date
        /// </summary>
        [JsonProperty("createdDate")]
        public DateTime? CreatedDate { get; set; }
    }
}