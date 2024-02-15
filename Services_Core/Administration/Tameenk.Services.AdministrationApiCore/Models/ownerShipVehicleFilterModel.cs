using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    public class ownerShipVehicleFilterModel
    {
        [JsonProperty("vehicleId")]
        public long VehicleId { get; set; }

        [JsonProperty("nationalId")]
        public long NationalId { get; set; }
    }
}