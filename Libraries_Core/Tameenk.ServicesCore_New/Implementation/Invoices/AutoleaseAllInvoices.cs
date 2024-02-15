using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Invoices
{
    public class AutoleaseAllInvoices
    {
        [JsonProperty("ReferenceId")]
        public string ReferenceId { get; set; }

        
        [JsonProperty("policyFileId")]
        public Guid? PolicyFileId { get; set; }

        [JsonProperty("FilePath")]
        public string FilePath { get; set; }

        [JsonProperty("InvoiceNo")]
        public int InvoiceNo { get; set; }


        [JsonProperty("InvoiceId")]
        public int InvoiceId { get; set; }



    }
}
