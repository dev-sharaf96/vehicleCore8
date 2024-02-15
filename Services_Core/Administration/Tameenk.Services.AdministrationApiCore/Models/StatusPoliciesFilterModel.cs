using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    /// <summary>
    /// Status Policies filter
    /// use class to filter Status policies
    /// </summary>
    [JsonObject("statusPoliciesFilter")]
    public class StatusPoliciesFilterModel
    {
        /// <summary>
        /// invoice No
        /// </summary>
        [JsonProperty("invoiceNo")]
        public int? InvoiceNo { get; set; }

        /// <summary>
        /// National id ( NIN)
        /// </summary>
        [JsonProperty("nationalId")]
        public string NationalId { get; set; }

        /// <summary>
        /// insured first name ar
        /// </summary>
        [JsonProperty("insuredFirstNameAr")]
        public string InsuredFirstNameAr { get; set; }


        /// <summary>
        /// insured last name ar
        /// </summary>
        [JsonProperty("insuredLastNameAr")]
        public string InsuredLastNameAr { get; set; }

        /// <summary>
        /// insured email
        /// </summary>
        [JsonProperty("insuredEmail")]
        public string InsuredEmail { get; set; }


        /// <summary>
        /// policy No
        /// </summary>
        [JsonProperty("policyNo")]
        public string PolicyNo { get; set; }

        /// <summary>
        /// Sequence No to vehicle
        /// </summary>
        [JsonProperty("sequenceNo")]
        public string SequenceNo { get; set; }

        /// <summary>
        /// Custom no to vehicle
        /// </summary>
        [JsonProperty("customNo")]
        public string CustomNo { get; set; }

        /// <summary>
        /// Reference No
        /// </summary>
        [JsonProperty("referenceNo")]
        public string ReferenceNo { get; set; }
    }
}