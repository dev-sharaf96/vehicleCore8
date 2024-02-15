using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.Policy.Components
{
    /// <summary>
    /// Represent the invoice model.
    /// </summary>
    [JsonObject("invoice")]
    public class InvoiceModel
    {
        /// <summary>
        /// identifier.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Invoice No.
        /// </summary>
        [JsonProperty("invoiceNo")]
        public int InvoiceNo { get; set; }

        /// <summary>
        /// Invoice Date.
        /// </summary>
        [JsonProperty("invoiceDate")]
        public DateTime InvoiceDate { get; set; }

        /// <summary>
        /// Invoice Due Date.
        /// </summary>
        [JsonProperty("invoiceDueDate")]
        public DateTime InvoiceDueDate { get; set; }

        /// <summary>
        /// User Id.
        /// </summary>
        [JsonProperty("userId")]
        public string UserId { get; set; }

        /// <summary>
        /// Reference Id.
        /// </summary>
        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

        /// <summary>
        /// Insurance Type Code.
        /// </summary>
        [JsonProperty("insuranceTypeCode")]
        public short? InsuranceTypeCode { get; set; }

        /// <summary>
        /// Insurance Company Id.
        /// </summary>
        [JsonProperty("insuranceCompanyId")]
        public int? InsuranceCompanyId { get; set; }

        /// <summary>
        /// Policy Id.
        /// </summary>
        [JsonProperty("policyId")]
        public int? PolicyId { get; set; }

        /// <summary>
        /// Product Price.
        /// </summary>
        [JsonProperty("productPrice")]
        public decimal? ProductPrice { get; set; }

        /// <summary>
        /// Extra Premium Price.
        /// </summary>
        [JsonProperty("extraPremiumPrice")]
        public decimal? ExtraPremiumPrice { get; set; }

        /// <summary>
        /// Discount.
        /// </summary>
        [JsonProperty("discount")]
        public decimal? Discount { get; set; }

        /// <summary>
        /// Fees.
        /// </summary>
        [JsonProperty("fees")]
        public decimal? Fees { get; set; }

        /// <summary>
        /// Vat.
        /// </summary>
        [JsonProperty("vat")]
        public decimal? Vat { get; set; }

        /// <summary>
        /// Sub Total Price.
        /// </summary>
        [JsonProperty("subTotalPrice")]
        public decimal? SubTotalPrice { get; set; }

        /// <summary>
        /// Total Price.
        /// </summary>
        [JsonProperty("totalPrice")]
        public decimal? TotalPrice { get; set; }

        /// <summary>
        /// user.
        /// </summary>
        [JsonProperty("user")]
        public UserModel User { get; set; }

        /// <summary>
        /// Insurance Company Name Ar. 
        /// </summary>
        [JsonProperty("insuranceCompanyNameAr")]
        public string InsuranceCompanyNameAr { get; set; }

        /// <summary>
        /// Insurance Company Name En . 
        /// </summary>
        [JsonProperty("insuranceCompanyNameEn")]
        public string InsuranceCompanyNameEn { get; set; }
    }
}