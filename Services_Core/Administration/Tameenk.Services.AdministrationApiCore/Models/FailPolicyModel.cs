using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.AdministrationApi.Models
{
    /// <summary>
    /// Fail policy Model
    /// </summary>
    [JsonObject("failPolicy")]
    public class FailPolicyModel
    {
        /// <summary>
        /// insurance Company En
        /// </summary>
        [JsonProperty("insuranceCompanyEn")]
        public string InsuranceCompanyEn { get; set; }


        /// <summary>
        /// insurance Company Ar
        /// </summary>
        [JsonProperty("insuranceCompanyAr")]
        public string InsuranceCompanyAr { get; set; }

        /// <summary>
        /// Max tries
        /// </summary>
        [JsonProperty("maxTries")]
        public int MaxTries { get; set; }

        /// <summary>
        /// IBAN in checkOut
        /// </summary>
        [JsonProperty("iban")]
        public string IBAN { get; set; }

        /// <summary>
        /// Reference No
        /// </summary>
        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }


        /// <summary>
        /// Create date
        /// </summary>
        [JsonProperty("createDate")]
        public  DateTime CreateDate { get; set; }


        /// <summary>
        /// Policy Status model
        /// </summary>
        [JsonProperty("policyStatus")]
        public PolicyStatusModel PolicyStatus { get; set; }

        
        /// <summary>
        /// User Email
        /// </summary>
        [JsonProperty("userEmail")]
        public string UserEmail { get; set; }     

        /// <summary>
        /// Invoice
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
        public short? VehiclePlateNumber { get; set; }

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
        /// Insured Phone
        /// </summary>
        [JsonProperty("insuredPhone")]
        public string InsuredPhone { get; set; }
      

        /// <summary>
        /// Vehicle
        /// </summary>
        [JsonProperty("vehicle")]
        public VehicleModel Vehicle { get; set; }

        /// <summary>
        /// Error Description ( reason of fail) 
        /// </summary>
        [JsonProperty("errorDescription")]
        public string ErrorDescription { get; set; }

        /// <summary>
        /// Service Request from insurance Company
        /// </summary>
        [JsonProperty("serviceRequest")]
        public string ServiceRequest { get; set; }

        /// <summary>
        /// Service Response from insurance company
        /// </summary>
        [JsonProperty("serviceResponse")]
        public string ServiceResponse { get; set; }
        [JsonProperty("address")]
        public AddressModel addressModel { get; set;}

    }
}