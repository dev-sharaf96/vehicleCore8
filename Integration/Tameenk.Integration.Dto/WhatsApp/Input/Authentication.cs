using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto
{
    public class Authentication
    {
        [JsonProperty("productToken")]
        public string ProductToken { get; set; }

    }
}
