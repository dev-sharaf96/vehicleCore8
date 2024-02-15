using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    /// <summary>
    /// Fail policy Filter Model
    /// </summary>

    [JsonObject("FailPolicyFilter")]
    public class FailPolicyFilterModel
    {
        /// <summary>
        /// Insured Phone
        /// </summary>
        [JsonProperty("insuredPhone")]
        public string InsuredPhone { get; set; }

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
        /// end Date
        /// </summary>
        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// start date
        /// </summary>
        [JsonProperty("startDate")]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Policy Status Id
        /// </summary>
        [JsonProperty("policyStatusId")]
        public int? PolicyStatusId { get; set; }

        /// <summary>
        /// Channel
        /// </summary>
        [JsonProperty("channel")]
        public int? Channel { get; set; }

        [JsonProperty("merchantId")]
        public string MerchantId { get; set; }

        [JsonProperty("pageIndex")]
        public int PageIndex { get; set; } = 1;

        [JsonProperty("pageSize")]
        public int pageSize { get; set; } = 10;

        [JsonProperty("sortOrder")]
        public bool SortOrder { get; set; } = false;

        [JsonProperty("isExcel")]
        public bool IsExcel { get; set; }
    }
}