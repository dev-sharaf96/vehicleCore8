using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Policies
{
   public class PoliciesForCancellationListingModel
    {

        /// <summary>
        /// Reference Id
        /// </summary>
        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

        /// <summary>
        /// Policy No
        /// </summary>
        [JsonProperty("policyNo")]
        public string PolicyNo { get; set; }

        /// <summary>
        /// Accident Report Number
        /// </summary>
        [JsonProperty("accidentReportNumber")]
        public string AccidentReportNumber { get; set; }

        /// <summary>
        /// Insured Id
        /// </summary>
        [JsonProperty("insuredId")]
        public string InsuredId { get; set; }

        /// <summary>
        /// Insured Mobile Number
        /// </summary>
        [JsonProperty("insuredMobileNumber")]
        public string InsuredMobileNumber { get; set; }

        /// <summary>
        /// Insured IBAN
        /// </summary>
        [JsonProperty("insuredIBAN")]
        public string InsuredIBAN { get; set; }

        /// <summary>
        /// Insured Bank Code
        /// </summary>
        [JsonProperty("insuredBankCode")]
        public int? InsuredBankCode { get; set; }

        /// <summary>
        /// Driver License Expiry Date
        /// </summary>
        [JsonProperty("driverLicenseExpiryDate")]
        public string DriverLicenseExpiryDate { get; set; }

        /// <summary>
        /// Driver License Type Code
        /// </summary>
        [JsonProperty("driverLicenseTypeCode")]
        public int? DriverLicenseTypeCode { get; set; }

        /// <summary>
        /// Accident Report
        /// </summary>
        [JsonProperty("accidentReport")]
        public byte[] AccidentReport { get; set; }

        /// <summary>
        /// Vehicle Id
        /// </summary>
        [JsonProperty("vehicleId")]
        public Guid VehicleId { get; set; }

        /// <summary>
        /// Company Id
        /// </summary>
        [JsonProperty("companyId")]
        public int? CompanyId { get; set; }

        /// <summary>
        /// Vehicle Id
        /// </summary>
        [JsonProperty("companyClassTypeName")]
        public string CompanyClassTypeName { get; set; }

        /// <summary>
        /// Vehicle Id
        /// </summary>
        [JsonProperty("companyNamespaceTypeName")]
        public string CompanyNamespaceTypeName { get; set; }

        
        [JsonProperty("companyName")]
        public string CompanyName { get; set; }

        [JsonProperty("isCancelled")]
        public bool IsCancelled { get; set; }


    }
}
