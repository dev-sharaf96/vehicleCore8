using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Dtos
{
    public class ActivateUserEmailModel
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("lang")]
        public string Lang { get; set; }
    }
}
