using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto
{
    public class Media
    {
        [JsonProperty("mediaName")]
        public string MediaName { get; set; }
        [JsonProperty("mediaUri")]
        public string MediaUri { get; set; }
        [JsonProperty("mimeType")]
        public string MimeType { get; set; }
    }
}
