using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    /// <summary>
    /// Payment Filter Model
    /// </summary>
    [JsonObject("paymentFilter")]
    public class PaymentFilterModel
    {
        /// <summary>
        /// Reference Id
        /// </summary>
        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }
        /// <summary>
        /// Payment method Id
        /// </summary>
        [JsonProperty("paymentMethodId")]
        public int? PaymentMethodId { get; set; }

        [JsonProperty("invoiceNo")]
        public string InvoiceNumber { get; set; }
        [JsonProperty("merchantId")]
        public string MerchantId { get; set; }
    }
}