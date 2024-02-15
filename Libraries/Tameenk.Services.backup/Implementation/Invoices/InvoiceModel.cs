using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Autoleasing.AdminApi
{
    /// <summary>
    /// Represent Invoice Model
    /// </summary>
    [JsonObject("invoice")]
    public class InvoiceModel
    {
        

        /// <summary>
        /// invoice id
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// invoice Number
        /// </summary>
        [JsonProperty("invoiceNo")]
        public int InvoiceNo { get; set; }


        /// <summary>
        /// invoice date
        /// </summary>
        [JsonProperty("invoiceDate")]
        public DateTime InvoiceDate { get; set; }

        /// <summary>
        /// invoice due date
        /// </summary>
        [JsonProperty("invoiceDueDate")]
        public DateTime InvoiceDueDate { get; set; }

        /// <summary>
        /// user id
        /// </summary>
        [JsonProperty("userId")]
        public string UserId { get; set; }

        /// <summary>
        /// reference Id
        /// </summary>
        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

        /// <summary>
        /// Insurance Type Code
        /// </summary>
        [JsonProperty("insuranceTypeCode")]
        public short? InsuranceTypeCode { get; set; }

        /// <summary>
        /// Insurance Company Id 
        /// </summary>
        [JsonProperty("insuranceCompanyId")]
        public int? InsuranceCompanyId { get; set; }

        /// <summary>
        /// Policy Id
        /// </summary>
        [JsonProperty("policyId")]
        public int? PolicyId { get; set; }

        /// <summary>
        /// Product Price
        /// </summary>
        [JsonProperty("productPrice")]
        public decimal? ProductPrice { get; set; }

        /// <summary>
        /// Extra Premium Price
        /// </summary>
        [JsonProperty("extraPremiumPrice")]
        public decimal? ExtraPremiumPrice { get; set; }

        /// <summary>
        /// Discount
        /// </summary>
        [JsonProperty("discount")]
        public decimal? Discount { get; set; }

        /// <summary>
        /// Fees
        /// </summary>
        [JsonProperty("fees")]
        public decimal? Fees { get; set; }

        /// <summary>
        /// Vat
        /// </summary>
        [JsonProperty("vat")]
        public decimal? Vat { get; set; }

        /// <summary>
        /// Sub Total Price
        /// </summary>
        [JsonProperty("subTotalPrice")]
        public decimal? SubTotalPrice { get; set; }

        /// <summary>
        /// Total Price
        /// </summary>
        [JsonProperty("totalPrice")]
        public decimal? TotalPrice { get; set; }


        /// <summary>
        ///  Creater Name
        /// </summary>
        [JsonProperty("createrName")]
        public string CreaterName { get; set; }


        /// <summary>
        /// Policy No
        /// </summary>
        [JsonProperty("policyNo")]
        public string PolicyNo { get; set; }

        /// <summary>
        /// Insurance Company Name
        /// </summary>
        [JsonProperty("insuranceCompanyNameEN")]
        public string InsuranceCompanyNameEN { get; set; }

        /// <summary>
        /// Insurance Company Name
        /// </summary>
        [JsonProperty("insuranceCompanyNameAR")]
        public string InsuranceCompanyNameAR { get; set; }

        /// <summary>
        /// Product Type EN
        /// </summary>
        [JsonProperty("productTypeEN")]
        public string ProductTypeEN { get; set; }


        /// <summary>
        /// Product Type AR
        /// </summary>
        [JsonProperty("productTypeAR")]
        public string ProductTypeAR { get; set; }
        [JsonProperty("paymentMethod")]
        public string PaymentMethod { get; set; }
    }
}