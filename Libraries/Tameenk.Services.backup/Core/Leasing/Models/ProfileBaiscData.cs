using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core.Leasing.Models
{
    public class ProfileBaiscData
    {
        [JsonProperty("leasingUserId")]
        public int LeasingUserId { get; set; }

        [JsonProperty("bankName")]
        public string BankName { get; set; }

        [JsonProperty("nin")]
        public string NIN { get; set; }

        [JsonProperty("fullNameAr")]
        public string FullNameAr { get; set; }

        [JsonProperty("fullNameEn")]
        public string FullNameEn { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("updatedEmail")]
        public string UpdatedEmail { get; set; }

        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("vehicleId")]
        public Guid VehicleId { get; set; }

        [JsonProperty("vehicleMaker")]
        public string VehicleMaker { get; set; }

        [JsonProperty("chassisNumber")]
        public string ChassisNumber { get; set; }

        [JsonProperty("policyFileId")]
        public Guid? PolicyFileId { get; set; }

        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

        [JsonProperty("policyNo")]
        public string PolicyNo { get; set; }

        [JsonProperty("externalId")]
        public string ExternalId { get; set; }
        [JsonProperty("parentExternalId")]
        public string ParentExternalId { get; set; }  
        [JsonProperty("parentReferenceId")]
        public string ParentReferenceId { get; set; }

        [JsonProperty("productId")]
        public Guid ProductId { get; set; }

        [JsonProperty("insuredBankCode")]
        public int? InsuredBankCode { get; set; }

        [JsonProperty("driverLicenseExpiryDate")]
        public string DriverLicenseExpiryDate { get; set; }

        [JsonProperty("driverLicenseTypeCode")]
        public short? DriverLicenseTypeCode { get; set; }

        [JsonProperty("companyId")]
        public int CompanyId { get; set; }

        [JsonProperty("companyClassTypeName")]
        public string CompanyClassTypeName { get; set; }

        [JsonProperty("companyNamespaceTypeName")]
        public string CompanyNamespaceTypeName { get; set; }

        public string Iban { get; set; }
    }
}