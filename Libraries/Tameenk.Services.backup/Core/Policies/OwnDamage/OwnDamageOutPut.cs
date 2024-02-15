using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core.Policies
{
   public class OwnDamageOutPut
    {
        [JsonProperty("data")]
        public List<OwnDamagePolicyInfo> Data { get; set; }
        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }
    }
}
