using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Providers.Wataniya.Dtos
{
    public class WataniyaTplQuotationResponseDto
    {
        public WataniyaTplQuotationResponseDetailsDto Details { get; set; }
        public int InsuranceCompanyCode { get; set; }
        public int QuoteReferenceNo { get; set; }
        public string RequestReferenceNo { get; set; }
        public bool Status { get; set; }
        public List<Errors> errors { get; set; }
    }

    public class WataniyaTplQuotationResponseDetailsDto
    {
        public int PolicyholderIdentityTypeCode { get; set; }
        public long PolicyholderID { get; set; }
        public int QuoteRequestSourceID { get; set; }
        public int PurposeofVehicleUseID { get; set; }
        public string FullName { get; set; }
        public string ArabicFirstName { get; set; }
        public string ArabicMiddleName { get; set; }
        public string ArabicLastName { get; set; }
        public string EnglishFirstName { get; set; }
        public string EnglishMiddleName { get; set; }
        public string EnglishLastName { get; set; }
        public string DateOfBirthG { get; set; }
        public string DateOfBirthH { get; set; }
        public short? Cylinders { get; set; }
        public int VehicleCapacity { get; set; }
        public short? PolicyholderNationalityID { get; set; }
        public int VehicleUniqueTypeID { get; set; }
        public int? VehicleSequenceNumber { get; set; }
        public long? VehicleCustomID { get; set; }
        public bool IsDriverDisabled { get; set; }
        public int? PolicyholderGender { get; set; }
        public short VehicleDriveRegionID { get; set; }
        public short VehicleDriveCityID { get; set; }
        public int CoverAgeLimitID { get; set; }
        public int VehiclePlateTypeID { get; set; }
        public short? VehiclePlateNumber { get; set; }
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
        public decimal PolicyAmount { get; set; }
        public decimal PolicyTaxableAmount { get; set; }
        public List<PremiumBreakDown> PremiumBreakDown { get; set; }
        public List<int> PolicyFeatures { get; set; }
        public int MaxLiability { get; set; }
        public string PolicyNumber { get; set; }
        public DateTime PolicyIssueDate { get; set; }
        public DateTime PolicyEffectiveDate { get; set; }
        public DateTime PolicyExpiryDate { get; set; }
        public int? MobileNo { get; set; }
        public int? BuildingNumber { get; set; }
        public string Street { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public int? ZipCode { get; set; }
        public int? AdditionalNumber { get; set; }
        public int? VehicleWeight { get; set; }
        public short? VehicleBodyCode { get; set; }
        public List<DriverDetails> DriverDetails { get; set; }
        public List<Discounts> Discounts { get; set; }
        public List<CustomizedParameter> CustomizedParameter { get; set; }

        // delete as new integration
        //public bool IsPrimaryDriver { get; set; }
        //public int? PrimaryDriverID { get; set; }
        //public string PrimaryDriverName { get; set; }
        //public string PrimaryDriverDateOfBirthG { get; set; }
        //public string PrimaryDriverDateOfBirthH { get; set; }
        //public int? PrimaryDriverGender { get; set; }
    }
}
