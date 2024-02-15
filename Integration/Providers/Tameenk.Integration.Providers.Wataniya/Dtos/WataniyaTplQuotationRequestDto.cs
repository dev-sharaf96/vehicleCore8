using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Providers.Wataniya.Dtos
{
    public class WataniyaTplQuotationRequestDto
    {
        public WataniyaTplQuotationRequesDetailstDto Details { get; set; }
        public short InsuranceCompanyCode { get; set; }
        public int InsuranceTypeID { get; set; }
        public string RequestReferenceNo { get; set; }
    }

    public class WataniyaTplQuotationRequesDetailstDto
    {
        public int? AdditionalNumber { get; set; }
        public string ArabicFirstName { get; set; }
        public string ArabicLastName { get; set; }
        public string ArabicMiddleName { get; set; }
        public int? ChildrenBelow16 { get; set; }
        public string City { get; set; }
        public int CoverAgeLimitID { get; set; }
        public List<CustomizedParameter> CustomizedParameter { get; set; }
        public string DateOfBirthG { get; set; }
        public string DateOfBirthH { get; set; }
        public string District { get; set; }
        public List<DriverDetails> DriverDetails { get; set; }
        public string Education { get; set; }
        public string EnglishFirstName { get; set; }
        public string EnglishLastName { get; set; }
        public string EnglishMiddleName { get; set; }
        public int? FirstPlateLetterID { get; set; }
        public string FullName { get; set; }
        public bool IsRenewal { get; set; }
        public bool IsScheme { get; set; }
        public int ManufactureYear { get; set; }
        public int? MaritalStatus { get; set; }
        public string MobileNo { get; set; }
        public string Occupation { get; set; }
        public string PolicyEffectiveDate { get; set; }
        public long PolicyHolderID { get; set; }
        public int? PolicyholderGender { get; set; }
        public string PolicyholderIDExpiry { get; set; }
        public int PolicyholderIdentityTypeCode { get; set; }
        public int? PolicyholderNCDCode { get; set; }
        public string PolicyholderNCDReference { get; set; }
        public short? PolicyholderNationalityID { get; set; }
        public int PurposeofVehicleUseID { get; set; }
        public short QuoteRequestSourceID { get; set; }
        public int? SecondPlateLetterID { get; set; }
        public string Street { get; set; }
        public int? ThirdPlateLetterID { get; set; }
        public int? Vehicle360Camera { get; set; }
        public int? VehicleABS { get; set; }
        public int? VehicleAdaptiveCruiseControl { get; set; }
        public int? VehicleAntitheftAlarm { get; set; }
        public int? VehicleAutoBraking { get; set; }
        public int VehicleCapacity { get; set; }
        public short VehicleColorCode { get; set; }
        public int? VehicleCruiseControl { get; set; }
        [JsonProperty("VehicleCustomID", NullValueHandling = NullValueHandling.Ignore)]
        public long? VehicleCustomID { get; set; }
        public short VehicleDriveCityID { get; set; }
        public short VehicleDriveRegionID { get; set; }
        public int? VehicleEngineSizeCC { get; set; }
        public int? VehicleExpectedMileageYear { get; set; }
        public int? VehicleFrontCamera { get; set; }
        public int? VehicleFrontSensors { get; set; }
        public short VehicleMakeCodeNIC { get; set; }
        public short? VehicleMakeCodeNajm { get; set; }
        public short VehicleMakeCode { get; set; }
        public string VehicleMakeTextNIC { get; set; }
        public short VehicleModelCodeNIC { get; set; }
        public short? VehicleModelCodeNajm { get; set; }
        public short VehicleModelCode { get; set; }
        public string VehicleModelTextNIC { get; set; }
        public int? VehicleNightParking { get; set; }
        public int VehiclePlateNumber { get; set; }
        [JsonProperty("VehiclePlateTypeID", NullValueHandling = NullValueHandling.Ignore)]
        public int? VehiclePlateTypeID { get; set; }
        public int? VehicleRearCamera { get; set; }
        public int? VehicleRearSensors { get; set; }
        public string VehicleRegistrationExpiryDate { get; set; }
        [JsonProperty("VehicleSequenceNumber", NullValueHandling = NullValueHandling.Ignore)]
        public long? VehicleSequenceNumber { get; set; }
        public int VehicleTransmission { get; set; }
        public int VehicleUniqueTypeID { get; set; }
        public string VehicleVIN { get; set; }
        public int? VehicleWeight { get; set; }
        public int? WorkCityID { get; set; }
        public string WorkCompanyName { get; set; }
        public int? ZipCode { get; set; }
        //public List<NajmCaseDetails> NajmCaseDetails { get; set; }
        public int? BuildingNumber { get; set; }
        public int Cylinders { get; set; }
        public short? VehicleRegistrationCityCode { get; set; }
        public short? VehicleBodyCode { get; set; }
        //public List<SchemeDetails> SchemeDetails { get; set; }
        [JsonProperty("SchemeDetails", NullValueHandling = NullValueHandling.Ignore)]
        public List<SchemeDetails> SchemeDetails { get; set; }

        // new properties in final integration
        public int? VehicleAxleWeight { get; set; }
        public int VehicleFireExtinguisher { get; set; }
        public int? VehicleMileage { get; set; }
        public string VehicleModifications { get; set; }

        // as per mohamed ghawati
        //public short VehicleUsagePercentage { get; set; }

        //// deleted from watania side as final integration
        //public bool IsDriverDisabled { get; set; }
        //public int? NajmNoOfAccidents { get; set; }
        //public string NajmDriverName { get; set; }
        //public string NCDFreeYears { get; set; }
        //public string NCDReference { get; set; }
        //public int VehicleSumInsured { get; set; }
        //public short RepairMethod { get; set; }
        //public bool IsPrimaryDriver { get; set; }
        //public int? PrimaryDriverID { get; set; }
        //public string PrimaryDriverName { get; set; }
        //public string PrimaryDriverDateOfBirthG { get; set; }
        //public string PrimaryDriverDateOfBirthH { get; set; }
        //public int? PrimaryDriverGender { get; set; }
    }
}
