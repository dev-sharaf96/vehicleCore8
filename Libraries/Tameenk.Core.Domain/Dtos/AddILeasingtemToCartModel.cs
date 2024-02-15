using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Tameenk.Core.Domain.Dtos
{
    public class AddILeasingtemToCartModel
    {
        [JsonProperty("externalId")]
        public string QuotaionRequestExternalId { get; set; }

        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

        [JsonProperty("productId")]
        public string ProductId { get; set; }

        [JsonProperty("selectedProductBenfitId")]
        public List<int> SelectedProductBenfitId { get; set; }

        [JsonProperty("selectedProductDriverId")]
        public List<int> SelectedProductDriverId { get; set; }

        [JsonProperty("lang")]
        public string Lang { get; set; }

        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("methodName")]
        public string MethodName { get; set; }

        public int LeasingServiceId { get; set; }
    }

    public class DriverModel
    {
        [JsonProperty("nationalId")]
        public string NationalId { get; set; }

        [JsonProperty("medicalConditionId")]
        public int MedicalConditionId { get; set; }

        [JsonProperty("violationIds")]
        public List<int> ViolationIds { get; set; }

        [JsonProperty("licenseExpiryMonth")]
        public byte? LicenseExpiryMonth { get; set; }

        [JsonProperty("licenseExpiryYear")]
        public short? LicenseExpiryYear { get; set; }

        [JsonProperty("edcuationId")]
        public int EducationId { get; set; }

        [JsonProperty("childrenBelow16Years")]
        public int ChildrenBelow16Years { get; set; }

        [JsonProperty("drivingPercentage")]
        public int DrivingPercentage { get; set; }

        [JsonProperty("birthDateMonth")]
        public byte BirthDateMonth { get; set; }

        [JsonProperty("birthDateYear")]
        public short BirthDateYear { get; set; }

        [JsonProperty("driverExtraLicenses")]
        public List<DriverExtraLicenseModel> DriverExtraLicenses { get; set; }

        [JsonProperty("driverNOALast5Years")]
        public int? DriverNOALast5Years { get; set; }

        [JsonProperty("driverWorkCityCode")]
        public int? DriverWorkCityCode { get; set; }

        [JsonProperty("driverWorkCity")]
        public string DriverWorkCity { get; set; }

        [JsonProperty("driverHomeCityCode")]
        public int? DriverHomeCityCode { get; set; }

        [JsonProperty("driverHomeCity")]
        public string DriverHomeCity { get; set; }

        [JsonProperty("isCompanyMainDriver")]
        public bool IsCompanyMainDriver { get; set; }

        [JsonProperty("relationShipId")]
        public int? RelationShipId { get; set; }
        [JsonProperty("mobileNo")]
        public string MobileNo { get; set; }
    }

    public class DriverExtraLicenseModel
    {
        [JsonProperty("countryId")]
        public short CountryId { get; set; }

        [JsonProperty("licenseYearsId")]
        public int LicenseYearsId { get; set; }
    }
}
