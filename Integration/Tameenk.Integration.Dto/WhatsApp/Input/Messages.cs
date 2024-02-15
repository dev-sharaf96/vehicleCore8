using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto
{
    public class Messages
    {
        [JsonProperty("authentication")]
        public Authentication authentication { get; set; }
        [JsonProperty("msg")]
        public MsgContent[] Msg { get; set; }
    }
}
