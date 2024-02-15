using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.PolicyApi.Models
{
    /// <summary>
    /// policy Model
    /// </summary>
    [JsonObject("policy")]
    public class PolicyModel
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
        /// Najm Status
        /// </summary>
        [JsonProperty("najmStatus")]
        public string NajmStatus { get; set; }

        /// <summary>
        /// Najm Status Code
        /// </summary>
        [JsonProperty("najmStatusCode")]
        public int NajmStatusCode { get; set; }

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
        [JsonProperty("checkOutDetailsId")]
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
        /// User name
        /// </summary>
        [JsonProperty("userName")]
        public string UserName { get; set; }

        /// <summary>
        /// company name Ar
        /// </summary>
        [JsonProperty("companyNameAr")]
        public string CompanyNameAr { get; set; }

        /// <summary>
        /// company name EN
        /// </summary>
        [JsonProperty("companyNameEn")]
        public string CompanyNameEn { get; set; }

        /// <summary>
        /// Policy is refunded
        /// </summary>
        [JsonProperty("isRefunded")]
        public bool IsRefunded { get; set; }

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
        /// policy Status Name En
        /// </summary>
        [JsonProperty("policyStatusNameEn")]
        public string PolicyStatusNameEn { set; get; }


        /*
        /// <summary>
        /// policy Status Name
        /// </summary>
        [JsonProperty("policyStatusName")]
        public string PolicyStatusName { set; get; }
        */ 

        /// <summary>
        /// policy Status Name Ar
        /// </summary>
        [JsonProperty("policyStatusNameAr")]
        public string PolicyStatusNameAr { set; get; }


        /// <summary>
        /// Vehicle Model Name
        /// </summary>
        [JsonProperty("vehicleModelName")]
        public string VehicleModelName { set; get; }

        /*/// <summary>
        /// Check out Detail
        /// </summary>
        [JsonProperty("checkoutDetail")]
        public CheckoutDetailModel CheckoutDetail { set; get; }*/


        /// <summary>
        /// Vehicle
        /// </summary>
        [JsonProperty("vehicle")]
        public VehicleModel Vehicle { get; set; }

        /// <summary>
        /// policy Status Name
        /// </summary>
        [JsonProperty("PolicyFileByte")]
        public byte[] PolicyFileByte { get; set; }

    }
}