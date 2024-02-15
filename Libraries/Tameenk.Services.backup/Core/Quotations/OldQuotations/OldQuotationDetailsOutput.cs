using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core.Quotations
{
    public class OldQuotationDetailsOutput
    {
        [JsonProperty("data")]
        public List<OldQuotationDetails> Data { get; set; }

        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }
    }
}
