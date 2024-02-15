using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto
{
    public class RichContent
    {
        public RichContent()
        {
            this.conversation = new Conversation[1];
        }
        [JsonProperty("conversation")]
        public Conversation[] conversation { get; set; }
    }
}
