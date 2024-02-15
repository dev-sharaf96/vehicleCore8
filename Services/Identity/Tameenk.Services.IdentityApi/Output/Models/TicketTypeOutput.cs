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
    [JsonObject("TicketTypeOutput")]
    public class TicketTypeOutput
    {
        /// <summary>
        /// Type Id
        /// </summary>
        [JsonProperty("typeId")]
        public int TypeId { get; set; }

        /// <summary>
        /// Ticket Type Name
        /// </summary>
        [JsonProperty("ticketTypeName")]
        public string TicketTypeName { get; set; }

        
    }
}