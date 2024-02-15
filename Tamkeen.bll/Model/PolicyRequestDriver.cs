using System;

namespace Tamkeen.bll.Model
{
    public class PolicyRequestDriver
    {
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

        //NOTE: this field need to be set from ELM (Yakeen)
        public int? MaritalStatusCode { get; set; }
        //NOTE: this field need to be set from ELM (Yakeen)
        public int? NumOfChildsUnder16 { get; set; }
        //NOTE: this field need to be set from ELM (Yakeen)
        public string Occupation { get; set; }

        //NOTE: this field need to be set from N (Najm)
        public int? SaudiLicenseHeldYears { get; set; }

        //NOTE: this field need to be set from N (Najm)
        public int? EligibleForNoClaimsDiscountYears { get; set; }

        //NOTE: this field need to be set from N (Najm)
        public int? NumOfFaultAccidentInLast5Years { get; set; }

        //NOTE: this field need to be set from N (Najm)
        public int? NumOfFaultclaimInLast5Years { get; set; }

        //NOTE: this field need to be set from ELM (Yakeen)
        public string RoadConvictions { get; set; }

        public int? DrivingLicenseTypeCode { get; set; }
    }
}
