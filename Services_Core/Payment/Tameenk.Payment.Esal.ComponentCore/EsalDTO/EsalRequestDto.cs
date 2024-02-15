using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Payment.Esal.Component
{
    public class EsalRequestDto
    {
        [JsonProperty("supplierId")]
        public string SupplierId { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        //public string GroupReferenceNumber { get; set; }
        //public string TotalInvoices { get; set; }
        [JsonProperty("invoices")]
        public List<InvoiceRequestDto> Invoices { get; set; }
    }
}
