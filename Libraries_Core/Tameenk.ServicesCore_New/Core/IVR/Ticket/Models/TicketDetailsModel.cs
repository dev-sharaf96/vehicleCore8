using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core.IVR
{
    public class TicketDetailsModel
    {
        [JsonProperty("ticketNo")]
        public int TicketNo { get; set; }

        [JsonProperty("ticketStatusId")]
        public int TicketStatusId { get; set; }

        [JsonProperty("ticketStatusName")]
        public string TicketStatusName { get; set; }

        [JsonProperty("ticketTypeId")]
        public int TicketTypeId { get; set; }

        [JsonProperty("ticketTypeName")]
        public string TicketTypeName { get; set; }
    }
}
