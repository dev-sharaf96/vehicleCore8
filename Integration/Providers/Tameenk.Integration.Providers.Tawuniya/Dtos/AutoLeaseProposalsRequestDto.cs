using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Integration.Dto.Providers;

namespace Tameenk.Integration.Providers.Tawuniya.Dtos
{

    public class AutoLeaseProposalsRequest
    {

        [JsonProperty("RequestReferenceNo")]
        public string RequestReferenceNo { get; set; }

        [JsonProperty("SourceCode")]
        public int SourceCode { get; set; }

        [JsonProperty("UserName")]
        public string UserName { get; set; }

        [JsonProperty("SchemeCode")]
        public string SchemeCode { get; set; }

        [JsonProperty("Details")]
        public AutoLeaseProposalRequestDetails Details { get; set; }


    }


    public class AutoLeaseProposalRequestDetails
    {
        [JsonProperty("LessorNameEN")]
        public string LessorNameEN { get; set; }

        [JsonProperty("LessorID")]
        public long LessorID { get; set; }

        [JsonProperty("LessorBranch")]
        public int LessorBranch { get; set; }

        [JsonProperty("LessorNationalAddress")]
        public List<NationalAddress> LessorNationalAddress { get; set; }

        [JsonProperty("LessorContactPerson")]
        public string LessorContactPerson { get; set; }

        [JsonProperty("LessorContactNumber")]
        public string LessorContactNumber { get; set; }

        [JsonProperty("LessorIBAN")]
        public string LessorIBAN { get; set; }

        [JsonProperty("BankCode")]
        public string BankCode { get; set; }

        [JsonProperty("IsRenewal")]
        public bool IsRenewal { get; set; }


        [JsonProperty("NajmCaseDetails")]
        public string NajmCaseDetails { get; set; }

        [JsonProperty("PurposeofVehicleUseID")]
        public byte PurposeofVehicleUseID { get; set; }

        [JsonProperty("Cylinders")]
        public string Cylinders { get; set; }

        [JsonProperty("VehicleMileage")]
        public int? VehicleMileage { get; set; }

        [JsonProperty("VehicleExpectedMileageYear")]
        public int? VehicleExpectedMileageYear { get; set; }

        [JsonProperty("VehicleEngineSizeCC")]
        public float? VehicleEngineSizeCC { get; set; }

        [JsonProperty("VehicleTransmission")]
        public int? VehicleTransmission { get; set; }

        [JsonProperty("VehicleNightParking")]
        public byte VehicleNightParking { get; set; }

        [JsonProperty("VehicleCapacity")]
        public int VehicleCapacity { get; set; }

        [JsonProperty("VehicleMakeCodeNIC")]
        public Int16 VehicleMakeCodeNIC { get; set; }

        [JsonProperty("VehicleMakeTextNIC")]
        public string VehicleMakeTextNIC { get; set; }

        [JsonProperty("VehicleModelCodeNIC")]
        public Int16 VehicleModelCodeNIC { get; set; }

        [JsonProperty("VehicleModelTextNIC")]
        public string VehicleModelTextNIC { get; set; }

        [JsonProperty("ManufactureYear")]
        public int ManufactureYear { get; set; }

        [JsonProperty("VehicleColorCode")]
        public Int16 VehicleColorCode { get; set; }

        [JsonProperty("VehicleModifications")]
        public string VehicleModifications { get; set; }

        [JsonProperty("VehicleSumInsured")]
        public int VehicleSumInsured { get; set; }

        [JsonProperty("RepairMethod")]
        public int RepairMethod { get; set; }
        [JsonProperty("LesseeID")]
        public long LesseeID { get; set; }

        [JsonProperty("FullName")]
        public string FullName { get; set; }

        [JsonProperty("ArabicFirstName")]
        public string ArabicFirstName { get; set; }
        [JsonProperty("ArabicMiddleName")]
        public string ArabicMiddleName { get; set; }

        [JsonProperty("ArabicLastName")]
        public string ArabicLastName { get; set; }

        [JsonProperty("EnglishFirstName")]
        public string EnglishFirstName { get; set; }

        [JsonProperty("EnglishMiddleName")]
        public string EnglishMiddleName { get; set; }

        [JsonProperty("EnglishLastName")]
        public string EnglishLastName { get; set; }

        [JsonProperty("LesseeNationalityID")]
        public short? LesseeNationalityID { get; set; }

        [JsonProperty("VehicleUsagePercentage")]
        public Int16 VehicleUsagePercentage { get; set; }

        [JsonProperty("LesseeOccupation")]
        public string LesseeOccupation { get; set; }

        [JsonProperty("LesseeEducation")]
        public string LesseeEducation { get; set; }

        [JsonProperty("LesseeChildrenBelow16")]
        public int LesseeChildrenBelow16 { get; set; }

        [JsonProperty("LesseeWorkCompanyName")]
        public string LesseeWorkCompanyName { get; set; }

        [JsonProperty("LesseeWorkCityID")]
        public string LesseeWorkCityID { get; set; }

        [JsonProperty("CountriesValidDrivingLicense")]
        public List<CountriesValidDrivingLicense> CountriesValidDrivingLicense { get; set; }
        [JsonProperty("LesseeNoOfClaims")]
        public string LesseeNoOfClaims { get; set; }

        [JsonProperty("LesseeTrafficViolationsCode")]
        public string LesseeTrafficViolationsCode { get; set; }

        [JsonProperty("LesseeHealthConditionsCode")]
        public string LesseeHealthConditionsCode { get; set; }
        [JsonProperty("LesseeDateOfBirthG")]
        public string LesseeDateOfBirthG { get; set; }
        [JsonProperty("LesseeDateOfBirthH")]
        public string LesseeDateOfBirthH { get; set; }

        [JsonProperty("LesseeGender")]
        public byte LesseeGender { get; set; }

        [JsonProperty("LesseeMaritalStatus")]
        public byte? LesseeMaritalStatus { get; set; }

        [JsonProperty("LesseeLicenseType")]
        public byte LesseeLicenseType { get; set; }
        [JsonProperty("LesseeLicenseOwnYears")]
        public byte LesseeLicenseOwnYears { get; set; }
        [JsonProperty("LesseeNCDCode")]
        public int LesseeNCDCode { get; set; }

        [JsonProperty("LesseeNCDReference")]
        public string LesseeNCDReference { get; set; }
        [JsonProperty("LesseeNoOfAccidents")]
        public int? LesseeNoOfAccidents { get; set; }
        [JsonProperty("LesseeNationalAddress")]
        public List<NationalAddress> LesseeNationalAddress { get; set; }
        [JsonProperty("DriverDetails")]
        public List<DriverDetails> DriverDetails { get; set; }

    }


    public class NationalAddress
    {
        [JsonProperty("BuildingNumber")]
        public int? BuildingNumber { get; set; }

        [JsonProperty("Street")]
        public string Street { get; set; }
        [JsonProperty("District")]
        public string District { get; set; }
        [JsonProperty("City")]
        public string City { get; set; }
        [JsonProperty("ZipCode")]
        public int? ZipCode { get; set; }
        [JsonProperty("AdditionalNumber")]
        public int? AdditionalNumber { get; set; }
    }

    public class CountriesValidDrivingLicense
    {

        [JsonProperty("DrivingLicenseCountryID")]
        public int DrivingLicenseCountryID { get; set; }

        [JsonProperty("DriverLicenseYears")]
        public int DriverLicenseYears { get; set; }
    }

    public class DriverDetails
    {
        [JsonProperty("DriverID")]
        public long DriverID { get; set; }
        [JsonProperty("DriverFullName")]
        public string DriverFullName { get; set; }
        [JsonProperty("ArabicFirstName")]
        public string ArabicFirstName { get; set; }
        [JsonProperty("ArabicMiddleName")]
        public string ArabicMiddleName { get; set; }
        [JsonProperty("ArabicLastName")]
        public string ArabicLastName { get; set; }
        [JsonProperty("EnglishFirstName")]
        public string EnglishFirstName { get; set; }
        [JsonProperty("EnglishMiddleName")]
        public string EnglishMiddleName { get; set; }
        [JsonProperty("EnglishLastName")]
        public string EnglishLastName { get; set; }
        [JsonProperty("DriverRelation")]
        public string DriverRelation { get; set; }
        [JsonProperty("DriverNationalityID")]
        public int DriverNationalityID { get; set; }
        [JsonProperty("VehicleUsagePercentage")]
        public int VehicleUsagePercentage { get; set; }
        [JsonProperty("DriverOccupation")]
        public string DriverOccupation { get; set; }
        [JsonProperty("DriverEducation")]
        public string DriverEducation { get; set; }
        [JsonProperty("DriverChildrenBelow16")]
        public string DriverChildrenBelow16 { get; set; }
        [JsonProperty("DriverWorkCompanyName")]
        public string DriverWorkCompanyName { get; set; }
        [JsonProperty("DriverWorkCityID")]
        public string DriverWorkCityID { get; set; }
        [JsonProperty("CountriesValidDrivingLicense")]
        public CountriesValidDrivingLicense CountriesValidDrivingLicense { get; set; }
        [JsonProperty("DriverNoOfClaims")]
        public string DriverNoOfClaims { get; set; }
        [JsonProperty("DriverTrafficViolationsCode")]
        public string DriverTrafficViolationsCode { get; set; }
        [JsonProperty("DriverHealthConditionsCode")]
        public string DriverHealthConditionsCode { get; set; }
        [JsonProperty("DriverDateOfBirthG")]
        public string DriverDateOfBirthG { get; set; }
        [JsonProperty("DriverDateOfBirthH")]
        public string DriverDateOfBirthH { get; set; }
        [JsonProperty("DriverGender")]
        public int DriverGender { get; set; }
        [JsonProperty("DriverMaritalStatus")]
        public int DriverMaritalStatus { get; set; }
        [JsonProperty("DriverHomeAddressCity")]
        public string DriverHomeAddressCity { get; set; }
        [JsonProperty("DriverHomeAddress")]
        public string DriverHomeAddress { get; set; }
        [JsonProperty("DriverLicenseType")]
        public int DriverLicenseType { get; set; }
        [JsonProperty("DriverLicenseOwnYears")]
        public int DriverLicenseOwnYears { get; set; }
        [JsonProperty("DriverNoOfAccidents")]
        public int DriverNoOfAccidents { get; set; }
       
    }

}
