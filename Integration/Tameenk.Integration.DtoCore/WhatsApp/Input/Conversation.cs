using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto
{
    public class Conversation
    {
        public Conversation()
        {
            this.Template = new Template();
        }
        [JsonProperty("template")]
        public Template Template { get; set; }

    }
}
