using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tameenk.Services.Implementation.Policies
{
    public class AutoleaseCancelledPoliciesFilter
    {
        /// <summary>
        /// Reference No
        /// </summary>
        [JsonProperty("referenceNo")]
        public string ReferenceNo { get; set; }

        /// <summary>
        /// policy No
        /// </summary>
        [JsonProperty("policyNo")]
        public string PolicyNo { get; set; }

        /// <summary>
        /// Start Date
        /// </summary>
        [JsonProperty("startDate")]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Start Date
        /// </summary>
        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// NajmStatus Id
        /// </summary>
        [JsonProperty("najmStatusId")]
        public int? NajmStatusId { get; set; }

        /// <summary>
        /// Invoice No
        /// </summary>
        [JsonProperty("invoiceNo")]
        public int? InvoiceNo { get; set; }

        /// <summary>
        /// Insurance Company Id
        /// </summary>
        [JsonProperty("insuranceCompanyId")]
        public int? InsuranceCompanyId { get; set; }

        /// <summary>
        /// Insurance Company Id
        /// </summary>
        [JsonProperty("bankId")]
        public int? BankId { get; set; }
        [JsonProperty("email")]
        public string UserEmail { get; set; }

        [JsonProperty("export")]
        public int? Export { get; set; }

        [JsonProperty("insurredId")]
        public string InsurredId { get; set; }
    }
}