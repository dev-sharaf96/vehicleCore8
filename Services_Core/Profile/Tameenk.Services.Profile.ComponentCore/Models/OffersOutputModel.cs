using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Profile.Component
{
    public class OffersOutputModel
    {
        public bool HasActivePolicy { get; set; }
        public DateTime? ExpiryDate { get; set; }

        [JsonProperty("hashed")]
        public string Hashed { get; set; }
    }
}
