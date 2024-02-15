using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto
{
    public class MsgContent
    {
        public MsgContent()
        {
            this.body = new Body();
            this.to = new To[1];
        }
        [JsonProperty("body")]
        public Body body { get; set; }
        [JsonProperty("to")]
        public To[] to { get; set; }
        [JsonProperty("from")]
        public string From { get; set; }
        [JsonProperty("allowedChannels")]
        public string[] AllowedChannels { get; set; }
        [JsonProperty("richContent")]
        public RichContent richContent { get; set; }
    }
}
