using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Tameenk.Services.QuotationNew.Components
{
    public class QuotationNewDriverDto
    {
        public QuotationNewDriverDto()
        {
            DriverLicenses = new List<QuotationNewLicenseDto>();
            DriverViolations = new List<QuotationNewViolationDto>();
        }
        public int? DriverTypeCode { get; set; }
        public long DriverId { get; set; }
        public int? DriverIdTypeCode { get; set; }
        public string DriverBirthDate { get; set; }
        public DateTime DriverBirthDateG { get; set; }
        public string DriverGenderCode { get; set; }
        public string DriverNationalityCode { get; set; }
        public string DriverFirstNameAr { get; set; }
        public string DriverMiddleNameAr { get; set; }
        public string DriverLastNameAr { get; set; }
        public string DriverFirstNameEn { get; set; }
        public string DriverMiddleNameEn { get; set; }
        public string DriverLastNameEn { get; set; }
        public int? DriverNCDFreeYears { get; set; }
        public string DriverNCDReference { get; set; }
        //NOTE: this field need to be set from ELM (Yakeen)
        public string DriverOccupation { get; set; }

        [JsonIgnore]
        public int Age { get; set; }
        [JsonIgnore]
        public int DriverAdditionalNumber { get; set; }

        [JsonIgnore]
        public int DriverUnitNo { get; set; }

        // fields need to be filled
        public string DriverSocialStatusCode { get; set; }
        [JsonProperty("DriverRelationship", NullValueHandling = NullValueHandling.Ignore)]
        public int? DriverRelationship { get; set; }
        public int? DriverDrivingPercentage { get; set; }
        public int? DriverEducationCode { get; set; }
        public string DriverOccupationCode { get; set; }
        public int? DriverMedicalConditionCode { get; set; }
        public int? DriverChildrenBelow16Years { get; set; }
        public string DriverHomeCityCode { get; set; }
        public string DriverHomeCity { get; set; }
        public string DriverWorkCityCode { get; set; }
        public string DriverWorkCity { get; set; }
        public int? DriverNOALast5Years { get; set; }
        public int? DriverNOCLast5Years { get; set; }
        [JsonProperty("DriverHomeAddress", NullValueHandling = NullValueHandling.Ignore)]
        public string DriverHomeAddress { get; set; }
        //list
        public List<QuotationNewLicenseDto> DriverLicenses { get; set; }
        public List<QuotationNewViolationDto> DriverViolations { get; set; }

        [JsonProperty("MobileNo", NullValueHandling = NullValueHandling.Ignore)]
        public string MobileNo { get; set; }

        [JsonProperty("DriverZipCode", NullValueHandling = NullValueHandling.Ignore)]
        public string DriverZipCode { get; set; }
    }
}
