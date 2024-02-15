using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto
{
    public class Body
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; }
    }
}
