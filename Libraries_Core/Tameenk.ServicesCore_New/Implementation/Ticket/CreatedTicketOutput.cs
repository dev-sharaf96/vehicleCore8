using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.Implementation.Policies
{
    /// <summary>
    /// Created Ticket Output
    /// </summary>
    [JsonObject("CreatedTicketOutput")]
    public class CreatedTicketOutput
    {
        [JsonProperty("ErrorCode")]
        public int ErrorCode { get; set; }

        [JsonProperty("ErrorDescription")]
        public string ErrorDescription { get; set; }

        [JsonProperty("TicketId")]
        public int TicketId { get; set; }
    }
}