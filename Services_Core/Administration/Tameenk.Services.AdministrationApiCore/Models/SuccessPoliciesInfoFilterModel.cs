using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    /// <summary>
    /// new success policies filter model
    /// </summary>
    public class SuccessPoliciesInfoFilterModel
    {
        /// <summary>
        /// PolicyNo
        /// </summary>
        [JsonProperty("policyNo")]
        public string PolicyNo { get; set; }

        /// <summary>
        /// InsuranceCompanyId
        /// </summary>
        [JsonProperty("insuranceCompanyId")]
        public int InsuranceCompanyId { get; set; }

        /// <summary>
        /// ReferenceNo
        /// </summary>
        [JsonProperty("referenceNo")]
        public string ReferenceNo { get; set; }

        /// <summary>
        /// NajmStatusId
        /// </summary>
        [JsonProperty("najmStatusId")]
        public int NajmStatusId { get; set; }

        /// <summary>
        /// PolicyNo
        /// </summary>
        [JsonProperty("invoiceNo")]
        public int InvoiceNo { get; set; }

        /// <summary>
        /// paymentMethodId
        /// </summary>
        [JsonProperty("paymentMethodId")]
        public int paymentMethodId { get; set; }
    }
}