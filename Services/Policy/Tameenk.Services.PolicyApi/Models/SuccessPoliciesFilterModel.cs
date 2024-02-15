using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.PolicyApi.Models
{
    /// <summary>
    /// Success Policies filter
    /// use class to filter success policies
    /// </summary>
    [JsonObject("successPoliciesFilter")]
    public class SuccessPoliciesFilterModel
    {
        /// <summary>
        /// insurance company id
        /// </summary>
        [JsonProperty("insuranceCompanyId")]
        public int? InsuranceCompanyId { get; set; }

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

        /// <summary>
        /// Product Type Id such as TPL & Comperhensive
        /// </summary>
        [JsonProperty("productTypeId")]
        public int? ProductTypeId { get; set; }

        
        /// <summary>
        /// Najm status
        /// </summary>
        [JsonProperty("najmStatusId")]
        public int? NajmStatusId { get; set; }

        /// <summary>
        /// end Date
        /// </summary>
        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }


        /// <summary>
        /// start date
        /// </summary>
        [JsonProperty("startDate")]
        public DateTime? StartDate { get; set; }
    }
}