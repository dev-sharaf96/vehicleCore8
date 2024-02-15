using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Tameenk.Services.AdministrationApi.Models
{
    public class SamaReportFilterModel
    {
        /// <summary>
        /// insurance company id
        /// </summary>
        [JsonProperty("insuranceCompanyId")]
        public int? InsuranceCompanyId { get; set; }

        /// <summary>
        /// To Date
        /// </summary>
        [JsonProperty("invoiceDateTo")]
        public DateTime? InvoiceDateTo { get; set; }

        /// <summary>
        /// From Date
        /// </summary>
        [JsonProperty("invoiceDateFrom")]
        public DateTime? InvoiceDateFrom { get; set; }

        /// <summary>
        /// Reference No
        /// </summary>
        [JsonProperty("referenceNo")]
        public string ReferenceNo { get; set; }

        [JsonProperty("policyHolderName")]
        public string PolicyHolderName { get; set; }

        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("policyNo")]
        public string PolicyNo { get; set; }

        [JsonProperty("productTypeCode")]
        public int? ProductTypeCode { get; set; }

        [JsonProperty("driverBirthDateFrom")]
        public DateTime? DriverBirthDateFrom { get; set; }

        [JsonProperty("driverBirthDateTo")]
        public DateTime? DriverBirthDateTo { get; set; }

        /// <summary>
        /// Is Excel
        /// </summary>
        [JsonProperty("isExcel")]
        public bool IsExcel { get; set; }

        /// <summary>
        /// channel id
        /// </summary>
        [JsonProperty("channel")]
        public int? Channel { get; set; }

        [JsonProperty("merchantId")]
        public string MerchantId { get; set; }

        [JsonProperty("isCorporate")]
        public short? IsCorporate { get; set; }
    }
}