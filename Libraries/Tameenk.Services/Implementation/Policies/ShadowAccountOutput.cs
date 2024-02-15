using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tameenk.Services.Implementation.Policies
{
    public class ShadowAccountOutput
    {
        [JsonProperty("DriverId")]
        public string DriverId { get; set; }
        [JsonProperty("CustomCardNumber")]
        public string CustomCardNumber { get; set; }
        [JsonProperty("SequenceNumber")]
        public string SequenceNumber { get; set; }
        [JsonProperty("NumberOfPolicies")]
        public int NumberOfPolicies { get; set; }
        [JsonProperty("TotalAmmount")]
        public decimal TotalAmmount { get; set; }
        [JsonProperty("ShadowAccount")]
        public decimal ShadowAccount { get; set; }

    }
}