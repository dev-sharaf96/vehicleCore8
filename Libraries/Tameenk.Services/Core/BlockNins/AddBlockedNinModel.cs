using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core.BlockNins
{
  public class AddBlockedNinModel
    {
        [JsonProperty("nationalId")]
        public string NationalId { get; set; }
        [JsonProperty("lang")]
        public string Lang { get; set; }
        [JsonProperty("blockReason")]
        public string BlockReason { get; set; }
    }
}
