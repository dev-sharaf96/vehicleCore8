
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    /// <summary>
    /// Edaat Filter Model
    /// </summary>
    [JsonObject("edaatFilter")]
    public class EdaatFilterModel
    {
        /// <summary>
        /// Reference Id
        /// </summary>
        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

        [JsonProperty("invoiceNo")]
        public string InvoiceNumber { get; set; }
        [JsonProperty("export")]
        public bool Export { get; set; }
    }
}