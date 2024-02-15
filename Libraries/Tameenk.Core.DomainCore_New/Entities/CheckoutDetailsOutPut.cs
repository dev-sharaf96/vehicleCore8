using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
    public class CheckoutDetailsOutPut
    {
        [JsonProperty("data")]
        public List<CheckOutInfo> Data { get; set; }
        [JsonProperty("file")]
        public byte[] File { get; set; }
        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }
    }
}
