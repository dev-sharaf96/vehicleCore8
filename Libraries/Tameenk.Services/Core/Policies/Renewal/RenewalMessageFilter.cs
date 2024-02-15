﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core.Policies.Renewal
{
    public class RenewalMessageFilter
    {
        [JsonProperty("vehicleId")]
        public string VehicleId { get; set; }

        [JsonProperty("nin")]
        public string Nin { get; set; }

        [JsonProperty("lang")]
        public string Lang { get; set; }

        [JsonProperty("pageNumber")]
        public int? PageNumber { get; set; }

        [JsonProperty("pageSize")]
        public int? PageSize { get; set; }
    }
}
