using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Tameenk.Core.Domain.Entities.VehicleInsurance;

namespace Tameenk.Integration.Dto.Providers
{
    public class AddDriverRequest
    {
        public string ReferenceId { set; get; }
        public string PolicyReferenceId { set; get; }
        public int? BankId { set; get; }
        public string PolicyNo { set; get; }
        public string AdditionStartDate { set; get; }
        public long DriverId { set; get; }
        public int? QuotationRequestId { set; get; }
        public int DriverIdTypeCode { set; get; }
        public string DriverBirthDate { get; set; }
        public string DriverNationalityCode { set; get; }
        public string DriverGenderCode { set; get; }
        public DateTime DriverBirthDateG { set; get; }
        public string DriverFirstNameAr { set; get; }
        public string DriverMiddleNameAr { set; get; }
        public string DriverLastNameAr { set; get; }
        public string DriverFirstNameEn { set; get; }
        public string DriverMiddleNameEn { set; get; }
        public string DriverLastNameEn { set; get; }
        public string DriverSocialStatusCode { set; get; }
        public string DriverOccupationCode { set; get; }
        public string DriverOccupation { set; get; }
        public int DriverDrivingPercentage { set; get; }
        public int DriverEducationCode { set; get; }
        public int DriverMedicalConditionCode { set; get; }
        public int DriverChildrenBelow16Years { set; get; }
        public string DriverHomeCityCode { set; get; }
        public string DriverHomeCity { set; get; }
        public string DriverWorkCityCode { set; get; }
        public string DriverWorkCity { set; get; }
        public int? DriverNOALast5Years { set; get; }
        public int? DriverNOCLast5Years { set; get; }
        public int DriverRelationship { set; get; }
        public List<LicenseDto> DriverLicenses { set; get; }
        public List<ViolationDto> DriverViolations { set; get; }
        [JsonIgnore]        public string MainDriverNin { set; get; }        [JsonIgnore]        public Vehicle Vehicle { set; get; }
        public string BankNin { set; get; }

    }     
    
}
