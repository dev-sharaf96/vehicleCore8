using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Providers.Wataniya.Dtos
{
    public class WataniyaCompDraftPolicyResponseDto
    {
        public WataniyaCompDraftPolicyResponseDetailsDto Details { get; set; }
        public int InsuranceCompanyCode { get; set; }
        public long PolicyReferenceNo { get; set; }
        public string PolicyRequestReferenceNo { get; set; }
        public long QuoteReferenceNo { get; set; }
        public string RequestReferenceNo { get; set; }
        public bool Status { get; set; }
        public List<Errors> errors { get; set; }
    }

    public class WataniyaCompDraftPolicyResponseDetailsDto
    {
        public int? AdditionalNumber { get; set; }
        public string ArabicFirstName { get; set; }
        public string ArabicMiddleName { get; set; }
        public string ArabicLastName { get; set; }
        public int? BuildingNumber { get; set; }
        public string City { get; set; }
        public int CoverAgeLimitID { get; set; }
        public short? Cylinders { get; set; }
        public string DateOfBirthH { get; set; }
        public int DeductibleAmount { get; set; }
        public List<Discounts> Discounts { get; set; }
        public string District { get; set; }
        public List<DraftPolicyDriverDetails> DriverDetails { get; set; }
        public string EnglishFirstName { get; set; }
        public string EnglishMiddleName { get; set; }
        public string EnglishLastName { get; set; }
        public int? FirstPlateLetterID { get; set; }
        public string FullName { get; set; }
        public bool IsRenewal { get; set; }
        public int ManufactureYear { get; set; }
        public string MobileNo { get; set; }
        public string Occupation { get; set; }
        public string PolicyEffectiveDate { get; set; }
        public string PolicyExpiryDate { get; set; }
        public long PolicyholderID { get; set; }
        public string PolicyIssueDate { get; set; }
        public string PolicyNumber { get; set; }
        public decimal PolicyPremium { get; set; }
        public List<PolicyPremiunFeatures> PolicyPremiumFeatures { get; set; }
        public short PolicyTitleID { get; set; }
        public int? PolicyholderGender { get; set; }
        public int PolicyholderIdentityTypeCode { get; set; }
        public short? PolicyholderNationalityID { get; set; }
        public List<PremiumBreakDown> PremiunBreakDown { get; set; }
        public int PurposeofVehicleUseID { get; set; }
        public short QuoteRequestSourceID { get; set; }
        public short RepairMethod { get; set; }
        public int? SecondPlateLetterID { get; set; }
        public string Street { get; set; }
        public int? ThirdPlateLetterID { get; set; }
        public short? VehicleBodyCode { get; set; }
        public int VehicleCapacity { get; set; }
        public short VehicleColorCode { get; set; }
        public short VehicleDriveCityID { get; set; }
        public short VehicleMakeCodeNIC { get; set; }
        public short? VehicleMakeCodeNajm { get; set; }
        public short VehicleMakeCode { get; set; }
        public string VehicleMakeTextNIC { get; set; }
        public short VehicleModelCodeNIC { get; set; }
        public short? VehicleModelCodeNajm { get; set; }
        public short VehicleModelCode { get; set; }
        public string VehicleModelTextNIC { get; set; }
        public short VehiclePlateNumber { get; set; }
        public int VehiclePlateTypeID { get; set; }
        public short? VehicleRegistrationCityCode { get; set; }
        public string VehicleRegistrationExpiryDate { get; set; }
        public int? VehicleSequenceNumber { get; set; }
        public int VehicleSumInsured { get; set; }
        public int VehicleUniqueTypeID { get; set; }
        public string VehicleVIN { get; set; }
        public int? VehicleWeight { get; set; }
        public int? ZipCode { get; set; }

        //public string PolicyholderIDExpiry { get; set; }
        //public string DateOfBirthG { get; set; }
        //public int? VehicleCUstomID { get; set; }
        //public bool IsDriverDisabled { get; set; }
        //public short VehicleDriveRegionID { get; set; }
        //public bool IsPrimaryDriver { get; set; }
        //public int? PrimaryDriverID { get; set; }
        //public string PrimaryDriverName { get; set; }
        //public string PrimaryDriverDateOfBirthG { get; set; }
        //public string PrimaryDriverDateOfBirthH { get; set; }
        //public int? PrimaryDriverGender { get; set; }
        //public List<CustomizedParameter> CustomizedParameter { get; set; }
    }

    public class DraftPolicyDriverDetails
    {
        public string DriverDateOfBirthH { get; set; }
        public int DriverGender { get; set; }
        public long DriverID { get; set; }
        public string DriverName { get; set; }
        public bool IsSamePolicyholderAddress { get; set; }
        public int VehicleUsagePercentage { get; set; }
    }
}
