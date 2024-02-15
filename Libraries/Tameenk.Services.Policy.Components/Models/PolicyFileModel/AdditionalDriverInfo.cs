using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Enums.Quotations;

namespace Tameenk.Services.Policy.Components
{
    public class AdditionalDriverInfo
    {

        #region Driver
        public Guid DriverId { get; set; }

        public bool DriverIsCitizen { get; set; }


        public string AdditionalDriverFirstNameEn { get; set; }
        public string AdditionalDriverFirstNameAr { get; set; }
        public string AdditionalDriverSecondNameEn { get; set; }
        public string AdditionalDriverSecondNameAr { get; set; }
        public string AdditionalDriverThirdNameEn { get; set; }
        public string AdditionalDriverThirdNameAr { get; set; }
        public string AdditionalDriverLastNameEn { get; set; }
        public string AdditionalDriverLastNameAr { get; set; }


        public string AdditionalDriverFullNameEn { get; set; }
        public string AdditionalDriverFullNameAr { get; set; }
        public string AdditionalDriverSubtribeName { get; set; }
        public DateTime AdditionalDriverCreatedDateTime { get; set; }
        public DateTime AdditionalDriverDateOfBirthG { get; set; }
        public short? AdditionalDriverNationalityCode { get; set; }
        public string AdditionalDriverDateOfBirthH { get; set; }
        public string AdditionalDriverNin { get; set; }
        public int AdditionalDriverGenderId { get; set; }
        public int? AdditionalDriverNCDFreeYears { get; set; }
        public string AdditionalDriverNCDReference { get; set; }
        public bool? AdditionalDriverIsSpecialNeed { get; set; }
        public string AdditionalDriverIdIssuePlace { get; set; }
        public string AdditionalDriverIdExpiryDate { get; set; }
        public int? AdditionalDriverDrivingPercentage { get; set; }
        public int? AdditionalDriverChildrenBelow16Years { get; set; }
        public int AdditionalDriverEducationId { get; set; }
        public int? AdditionalDriverSocialStatusId { get; set; }
        public int? AdditionalDriverOccupationId { get; set; }
        public int? MedicalConditionId { get; set; }
        public string AdditionalDriverResidentOccupation { get; set; }
        public long? AdditionalDriverCityId { get; set; }
        public long? AdditionalDriverWorkCityId { get; set; }
        public int? AdditionalDriverNOALast5Years { get; set; }
        public int? AdditionalDriverNOCLast5Years { get; set; }
        public string AdditionalDriverCityName { get; set; }
        public int? AdditionalDriverAddressId { get; set; }
        public string AdditionalDriverWorkCityName { get; set; }
        public string AdditionalDriverOccupationName { get; set; }
        public string AdditionalDriverEducationName { get; set; }
        public string AdditionalDriverSocialStatusName { get; set; }
        public string AdditionalDriverPostCode { get; set; }
        public string AdditionalDriverExtraLicenses { get; set; }
        public string AdditionalDriverLicenses { get; set; }
        public string AdditionalDriverViolations { get; set; }
        public int? AdditionalDriverViolationId { get; set; }
        public int? AdditionalDriverSaudiLicenseHeldYears { get; set; }
        public string AdditionalDriverOccupationCode { get; set; }
        public int? AdditionalDriverRelationShipId { get; set; }
        public string AdditionalDriverMobileNumber { get; set; }
        public Gender AdditionalDriverGender
        {
            get { return (Gender)AdditionalDriverGenderId; }
            set { AdditionalDriverGenderId = (int)value; }
        }
        public string AdditionalDriverOccupationAr { get; set; }
        public string AdditionalDriverOccupationEn { get; set; }
      
        public string AdditionalDriverCityNameAr { get; set; }
        public string AdditionalDriverCityNameEn { get; set; }
        public string AdditionalDriverWorkCityNameAr { get; set; }
        public string AdditionalDriverWorkCityNameEn { get; set; }
        public bool IsCitizen { get; set; }


        #endregion
    }
}
