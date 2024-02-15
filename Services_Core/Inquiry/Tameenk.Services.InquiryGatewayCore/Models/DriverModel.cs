using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tameenk.Services.InquiryGateway.Models
{
    [JsonObject("driver")]
    public class DriverModel
    {
        [Required]
        [JsonProperty("nationalId")]
        public string NationalId { get; set; }

        /// <summary>
        /// Driver medical condition id
        /// </summary>
        [JsonProperty("medicalConditionId")]
        public int MedicalConditionId { get; set; }

        /// <summary>
        /// List of driver violation identifiers.
        /// </summary>
        [JsonProperty("violationIds")]
        public List<int> ViolationIds { get; set; }


        /// <summary>
        /// The driving license expiry month.
        /// </summary>
        [JsonProperty("licenseExpiryMonth")]
        //[Required]
        //[Range(1, 12)]
        public byte? LicenseExpiryMonth { get; set; }

        /// <summary>
        /// The driving license expiry year.
        /// </summary>
        [JsonProperty("licenseExpiryYear")]
        //[Required]
        public short? LicenseExpiryYear { get; set; }


        /// <summary>
        /// Insured Education identifier.
        /// </summary>
        [Required]
        [JsonProperty("edcuationId")]
        public int EducationId { get; set; }

        /// <summary>
        /// Number of children under age 16 years.
        /// </summary>
        [JsonProperty("childrenBelow16Years")]
        public int ChildrenBelow16Years { get; set; }

        /// <summary>
        /// Driver percentage of using the vehicle related to other driver(s) useage.
        /// </summary>
        [JsonProperty("drivingPercentage")]
        public int DrivingPercentage { get; set; }

        /// <summary>
        /// Customer birthdate month.
        /// </summary>
        [JsonProperty("birthDateMonth")]
        [Required]
        [Range(1, 12)]
        public byte BirthDateMonth { get; set; }


        /// <summary>
        /// Customer birth date year.
        /// </summary>
        [JsonProperty("birthDateYear")]
        [Required]
        public short BirthDateYear { get; set; }

        [JsonProperty("driverExtraLicenses")]
        public List<DriverExtraLicenseModel> DriverExtraLicenses { get; set; }
    }
}