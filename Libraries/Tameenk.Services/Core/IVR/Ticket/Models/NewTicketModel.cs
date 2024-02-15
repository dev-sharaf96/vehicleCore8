using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core
{
    public class NewTicketModel
    {
        [JsonProperty("ticketTypeId")]
        public int TicketTypeId { get; set; }

        [JsonProperty("ticketSubTypeId")]
        public int? TicketSubTypeId { get; set; }

        [JsonProperty("vehicleId")]
        public string VehicleId { get; set; }

        [JsonProperty("nationalId")]
        public string NationalId { get; set; }

        [JsonProperty("sadadNo")]
        public int? SadadNo { get; set; }

        [JsonProperty("PhoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("sendToCallerPhone")]
        public bool SendToCallerPhone { get; set; } = false;
    }
}
