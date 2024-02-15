using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Inquiry.Components
{
    public class FailModel
    {
        [JsonProperty("rowNumber")]
        public int RowNumber { get; set; }

        [JsonProperty("nin")]
        public string Nin { get; set; }

        [JsonProperty("vehicleId")]
        public string VehicleId { get; set; }

        [JsonProperty("errorDescription")]
        public string ErrorDescription { get; set; }
    }
}
