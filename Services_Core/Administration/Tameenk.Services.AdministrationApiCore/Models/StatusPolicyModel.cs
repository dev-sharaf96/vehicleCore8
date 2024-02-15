using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    /// <summary>
    /// policy Model
    /// </summary>
    [JsonObject("statusPolicy")]
    public class StatusPolicyModel
    {
        /// <summary>
        /// Policy Status model
        /// </summary>
        [JsonProperty("policyStatus")]
        public PolicyStatusModel PolicyStatus { get; set; }

        /// <summary>
        /// Najm Status model
        /// </summary>
        [JsonProperty("najmStatusObj")]
        public NajmStatusModel NajmStatusObj { get; set; }

        /// <summary>
        /// Policy id
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Insurance Company ID
        /// </summary>
        [JsonProperty("insuranceCompanyID")]
        public int? InsuranceCompanyID { get; set; }

        /// <summary>
        /// Status Code
        /// </summary>
        [JsonProperty("statusCode")]
        public byte StatusCode { get; set; }

        /// <summary>
        /// Policy Number
        /// </summary>
        [JsonProperty("policyNo")]
        public string PolicyNo { get; set; }

        /// <summary>
        /// Policy Issue Date
        /// </summary>
        [JsonProperty("policyIssueDate")]
        public DateTime? PolicyIssueDate { get; set; }

        /// <summary>
        /// Policy Effective Date
        /// </summary>
        [JsonProperty("policyEffectiveDate")]
        public DateTime? PolicyEffectiveDate { get; set; }

        /// <summary>
        /// Policy Expiry date
        /// </summary>
        [JsonProperty("policyExpiryDate")]
        public DateTime? PolicyExpiryDate { get; set; }

        /// <summary>
        /// Checkout details id
        /// </summary>
        [JsonProperty("referenceId")]
        public string CheckOutDetailsId { get; set; }

        /// <summary>
        /// Policy File Id
        /// </summary>
        [JsonProperty("policyFileId")]
        public Guid? PolicyFileId { get; set; }

        /// <summary>
        /// User Email
        /// </summary>
        [JsonProperty("userEmail")]
        public string UserEmail { get; set; }

        /// <summary>
        /// Invoices
        /// </summary>
        [JsonProperty("invoices")]
        public ICollection<InvoiceModel> Invoices { get; set; }

        /// <summary>
        /// Invoices
        /// </summary>
        [JsonProperty("invoice")]
        public InvoiceModel Invoice { get; set; }

        /// <summary>
        /// Insured Full Name in Arabic
        /// </summary>
        [JsonProperty("insuredFullNameAr")]
        public string InsuredFullNameAr { get; set; }

        /// <summary>
        /// Insured Full Name in English
        /// </summary>
        [JsonProperty("insuredFullNameEn")]
        public string InsuredFullNameEn { get; set; }

        /// <summary>
        /// Insured Id
        /// </summary>
        [JsonProperty("insuredNIN")]
        public string InsuredNIN { get; set; }

        /// <summary>
        /// Vehicle Plate Number
        /// </summary>
        [JsonProperty("vehiclePlateNumber")]
        public string VehiclePlateNumber { get; set; }

        /// <summary>
        /// product type model
        /// </summary>
        [JsonProperty("productType")]
        public ProductTypeModel ProductTypeModel { get; set; }

        /// <summary>
        /// Vehicle Model Name
        /// </summary>
        [JsonProperty("vehicleModelName")]
        public string VehicleModelName { set; get; }

        /// <summary>
        /// insurance company name ar
        /// </summary>
        [JsonProperty("insuranceCompanyNameAr")]
        public string InsuranceCompanyNameAr { get; set; }

        /// <summary>
        /// Insured Phone
        /// </summary>
        [JsonProperty("insuredPhone")]
        public string InsuredPhone { get; set; }

        /// <summary>
        /// insurance company name en
        /// </summary>
        [JsonProperty("insuranceCompanyNameEn")]
        public string InsuranceCompanyNameEn { get; set; }

        /// <summary>
        /// Vehicle
        /// </summary>
        [JsonProperty("vehicle")]
        public VehicleModel Vehicle { get; set; }

        [JsonProperty("ImageBack")]
        public string ImageBack { get; set; }

        [JsonProperty("ImageFront")]
        public string ImageFront { get; set; }

        [JsonProperty("ImageBody")]
        public string ImageBody { get; set; }

        [JsonProperty("ImageLeft")]
        public string ImageLeft { get; set; }

        [JsonProperty("ImageRight")]
        public string ImageRight { get; set; }

        [JsonProperty("IsCancelled")]
        public bool IsCancelled { get; set; }

        [JsonProperty("CancelationDate")]
        public DateTime? CancelationDate { get; set; }
    }
}