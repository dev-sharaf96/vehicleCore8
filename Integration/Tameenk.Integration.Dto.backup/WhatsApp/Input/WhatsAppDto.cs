using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto
{
    public class WhatsAppDto
    {
        public WhatsAppDto()
        {
            this.message = new Messages();
            this.message.authentication = new Authentication();
            this.message.Msg = new MsgContent[4];
        }
        [JsonProperty("messages")]
        public Messages message { get; set; }
    }
}
