using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core.IVR
{
    public class RenewalQuotationRequestModel
    {
        [JsonProperty("vehicleId")]
        public string VehicleId { get; set; }

        [JsonProperty("nationalId")]
        public string NationalId { get; set; }

        [JsonProperty("externalId")]
        public string ExternalId { get; set; }
    }
}
