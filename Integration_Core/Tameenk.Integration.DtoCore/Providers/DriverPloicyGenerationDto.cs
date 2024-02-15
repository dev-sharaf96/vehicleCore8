using System;
using System.Collections.Generic;

namespace Tameenk.Integration.Dto.Providers
{
    public class DriverPloicyGenerationDto
    {
        public DriverPloicyGenerationDto()
        {
            DriverLicenses = new List<LicenseDto>();
            DriverViolations = new List<ViolationDto>();
        }
        public string NIN { get; set; }
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
        public string DriverFullNameAr { get; set; }
        public string DriverFullNameEn { get; set; }
        public string IsAgeLessThen21YearsAR { get; set; }
        public string IsAgeLessThen21YearsEN { get; set; }
        public string DriverFirstNameEn { get; set; }
        public string DriverMiddleNameEn { get; set; }
        public string DriverLastNameEn { get; set; }
        public int? DriverNCDFreeYears { get; set; }
        public string DriverNCDReference { get; set; }
        //NOTE: this field need to be set from ELM (Yakeen)
        public string DriverOccupation { get; set; }
        public int Age { get; set; }
        public int DriverAdditionalNumber { get; set; }
        public int DriverUnitNo { get; set; }
        // fields need to be filled
        public string DriverSocialStatusCode { get; set; }
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

        public string NCDPercentage { get; set; }
        public string NCDAmount { get; set; }

        //list
        public List<LicenseDto> DriverLicenses { get; set; }
        public List<ViolationDto> DriverViolations { get; set; }

    }
}
