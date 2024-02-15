using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core.Quotations
{
    public class OldQuotationDetailsFilter
    {
        [JsonProperty("nationalId")]
        public string NationalId { get; set; }

        [JsonProperty("sequenceNumber")]
        public string SequenceNumber { get; set; }

        [JsonProperty("referenceNo")]
        public string referenceNo { get; set; }
        
        [JsonProperty("pageNumber")]
        public int? PageNumber { get; set; }

        [JsonProperty("pageSize")]
        public int? PageSize { get; set; }

        [JsonProperty("isExport")]
        public bool isExport { get; set; }

    }
}
