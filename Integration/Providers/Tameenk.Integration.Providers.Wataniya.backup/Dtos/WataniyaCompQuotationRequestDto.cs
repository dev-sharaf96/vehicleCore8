using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Providers.Wataniya.Dtos
{
    public class WataniyaCompQuotationRequestDto
    {
        public WataniyaCompQuotationRequesDetailstDto Details { get; set; }
        public short InsuranceCompanyCode { get; set; }
        public int InsuranceTypeID { get; set; }
        public string RequestReferenceNo { get; set; }
    }

    public class WataniyaCompQuotationRequesDetailstDto
    {
        public int PolicyholderIdentityTypeCode { get; set; }
        public long PolicyHolderID { get; set; }
        public string PolicyholderIDExpiry { get; set; }
        public int PurposeofVehicleUseID { get; set; }
        public short QuoteRequestSourceID { get; set; }
        public string FullName { get; set; }
        public string ArabicFirstName { get; set; }
        public string ArabicMiddleName { get; set; }
        public string ArabicLastName { get; set; }
        public string EnglishFirstName { get; set; }
        public string EnglishMiddleName { get; set; }
        public string EnglishLastName { get; set; }
        public string DateOfBirthG { get; set; }
        public string DateOfBirthH { get; set; }
        public string Occupation { get; set; }
        public int Cylinders { get; set; }
        public int VehicleCapacity { get; set; }
        public short? PolicyholderNationalityID { get; set; }
        public int VehicleUniqueTypeID { get; set; }
        [JsonProperty("VehicleSequenceNumber", NullValueHandling = NullValueHandling.Ignore)]
        public long? VehicleSequenceNumber { get; set; }
        [JsonProperty("VehicleCustomID", NullValueHandling = NullValueHandling.Ignore)]
        public long? VehicleCustomID { get; set; }
        public int? PolicyholderGender { get; set; }
        public short VehicleDriveRegionID { get; set; }
        public short VehicleDriveCityID { get; set; }
        [JsonProperty("VehiclePlateTypeID", NullValueHandling = NullValueHandling.Ignore)]
        public int? VehiclePlateTypeID { get; set; }
        public int VehiclePlateNumber { get; set; }
        public int? FirstPlateLetterID { get; set; }
        public int? SecondPlateLetterID { get; set; }
        public int? ThirdPlateLetterID { get; set; }
        public short? VehicleMakeCodeNajm { get; set; }
        public short VehicleMakeCodeNIC { get; set; }
        public string VehicleMakeTextNIC { get; set; }
        public short VehicleMakeCode { get; set; }
        public short? VehicleModelCodeNajm { get; set; }
        public short VehicleModelCodeNIC { get; set; }
        public string VehicleModelTextNIC { get; set; }
        public short VehicleModelCode { get; set; }
        public int ManufactureYear { get; set; }
        public short VehicleColorCode { get; set; }
        public short? VehicleRegistrationCityCode { get; set; }
        public string VehicleVIN { get; set; }
        public string VehicleRegistrationExpiryDate { get; set; }
        public string PolicyEffectiveDate { get; set; }
        public short PolicyTitleID { get; set; }
        //public int? MobileNo { get; set; }
        public string MobileNo { get; set; }
        public int? BuildingNumber { get; set; }
        public string Street { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public int? ZipCode { get; set; }
        public int? AdditionalNumber { get; set; }
        public int? VehicleWeight { get; set; }
        public short? VehicleBodyCode { get; set; }
        public int CoverAgeLimitID { get; set; }
        public List<DriverDetails> DriverDetails { get; set; }
        public List<NajmCaseDetails> NajmCaseDetails { get; set; }
        public string NCDFreeYears { get; set; }
        public string NCDReference { get; set; }
        public int VehicleSumInsured { get; set; }
        public int RepairMethod { get; set; }
        //public bool IsPrimaryDriver { get; set; }
        //public int? PrimaryDriverID { get; set; }
        //public string PrimaryDriverName { get; set; }
        //public string PrimaryDriverDateOfBirthG { get; set; }
        //public string PrimaryDriverDateOfBirthH { get; set; }
        //public int? PrimaryDriverGender { get; set; }
        public bool IsRenewal { get; set; }
        public bool IsScheme { get; set; }
        [JsonProperty("SchemeDetails", NullValueHandling = NullValueHandling.Ignore)]
        public List<SchemeDetails> SchemeDetails { get; set; }
        public List<CustomizedParameter> CustomizedParameter { get; set; }
        public int? ChildrenBelow16 { get; set; }
        public string Education { get; set; }
        public int? MaritalStatus { get; set; }
        public int? PolicyholderNCDCode { get; set; }
        public string PolicyholderNCDReference { get; set; }
        public int? Vehicle360Camera { get; set; }
        public int? VehicleABS { get; set; }
        public int? VehicleAdaptiveCruiseControl { get; set; }
        public int? VehicleAntitheftAlarm { get; set; }
        public int? VehicleAutoBraking { get; set; }
        public int? VehicleCruiseControl { get; set; }
        public int? VehicleEngineSizeCC { get; set; }
        public int? VehicleExpectedMileageYear { get; set; }
        public int? VehicleFrontCamera { get; set; }
        public int? VehicleFrontSensors { get; set; }
        public string VehicleModifications { get; set; }
        public int? VehicleRearCamera { get; set; }
        public int? VehicleRearSensors { get; set; }
        public int? VehicleTransmission { get; set; }
        public int? WorkCityID { get; set; }
        public string WorkCompanyName { get; set; }
        public int? VehicleNightParking { get; set; }
        public string VehicleColorText { get; set; }


        // new properties as new integration
        public int? VehicleAxleWeight { get; set; }
        public short? VehicleFireExtinguisher { get; set; }
        public int? VehicleMileage { get; set; }
        //public short VehicleUsagePercentage { get; set; }


        // deleted as new integration
        //public bool IsDriverDisabled { get; set; }
        //public int? NajmNoOfAccidents { get; set; }
        //public string NajmDriverName { get; set; }
        //public int? VehicleEngineSizeCode { get; set; }
    }

    public class DriverDetails
    {
        public long DriverID { get; set; }
        public string DriverName { get; set; }
        [JsonProperty("DriverDateOfBirthG", NullValueHandling = NullValueHandling.Ignore)]
        public string DriverDateOfBirthG { get; set; }
        [JsonProperty("DriverDateOfBirthH", NullValueHandling = NullValueHandling.Ignore)]
        public string DriverDateOfBirthH { get; set; }
        public int DriverGender { get; set; }
        public int? DriverChildrenBelow16 { get; set; }
        public string DriverEducation { get; set; }
        public string DriverHomeAddress { get; set; }
        public string DriverHomeAddressCity { get; set; }
        [JsonProperty("DriverLicenseOwnYears", NullValueHandling = NullValueHandling.Ignore)]
        public int? DriverLicenseOwnYears { get; set; }
        [JsonProperty("DriverLicenseType", NullValueHandling = NullValueHandling.Ignore)]
        public int? DriverLicenseType { get; set; }
        public int? DriverMaritalStatus { get; set; }
        public int? DriverNCDCode { get; set; }
        public string DriverNCDReference { get; set; }
        public int? DriverNoOfAccidents { get; set; }
        public int? DriverNoOfClaims { get; set; }
        public string DriverOccupation { get; set; }
        [JsonProperty("DriverRelation", NullValueHandling = NullValueHandling.Ignore)]
        public int? DriverRelation { get; set; }
        public bool IsPolicyHolder { get; set; }
        public bool IsSamePolicyholderAddress { get; set; }
        public bool IsUser { get; set; }
        public int VehicleUsagePercentage { get; set; }

        // new properties
        public List<CountriesValidDrivingLicense> CountriesValidDrivingLicense { get; set; }
        public string DriverHealthConditionsCode { get; set; }
        public string DriverTrafficViolationsCode { get; set; }
        public int? DriverWorkCityID { get; set; }
        public string DriverWorkCompanyName { get; set; }
    }

    public class NajmCaseDetails
    {
        public string CaseNumber { get; set; }
        public string AccidentDate { get; set; }
        public string Liability { get; set; }
        public string DriverAge { get; set; }
        public string CarModel { get; set; }
        public string CarType { get; set; }
        public string DriverID { get; set; }
        public string SequenceNumber { get; set; }
        public string OwnerID { get; set; }
        public string EstimatedAmount { get; set; }
        public string DamageParts { get; set; }
        public string CauseOfAccident { get; set; }
    }

    public class SchemeDetails
    {
        public string SchemeRef { get; set; }
        public string IcSchemeRef { get; set; }
        public int? SchemeTypeID { get; set; }
        public int? PositionNameCode { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }
        public string Value3 { get; set; }
        public string Value4 { get; set; }
    }

    public class CustomizedParameter
    {
        public string Key { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }
        public string Value3 { get; set; }
        public string Value4 { get; set; }
    }

    public class CountriesValidDrivingLicense
    {
        public int DrivingLicenseCountryID { get; set; }
        public int DriverLicenseYears { get; set; }
    }
}
