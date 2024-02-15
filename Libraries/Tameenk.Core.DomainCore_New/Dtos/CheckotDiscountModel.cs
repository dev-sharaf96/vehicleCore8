using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Dtos
{
    public class CheckotDiscountModel
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("Channel")]
        public string Channel { get; set; }

        [JsonProperty("lang")]
        public string Lang { get; set; }

        [JsonProperty("vehicleId")]
        public string VehicleId { get; set; }

        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

        [JsonProperty("driverNin")]
        public string DriverNin { get; set; }
    }
}
