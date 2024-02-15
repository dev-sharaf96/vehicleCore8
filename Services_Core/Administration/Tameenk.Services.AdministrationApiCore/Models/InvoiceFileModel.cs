using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    [JsonObject("InvoiceFile")]
    public class InvoiceFileModel
    {
        /// <summary>
        /// invoice file id
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// invoice data in binary
        /// </summary>
        [JsonProperty("invoiceData")]
        public byte[] InvoiceData { get; set; }

    
    }
}