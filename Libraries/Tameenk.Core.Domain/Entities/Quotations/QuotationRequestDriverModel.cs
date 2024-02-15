using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Enums.Vehicles;

namespace Tameenk.Core.Domain.Entities
{
    public class QuotationRequestDriverModel
    {
        public int QuotationRequestId { get; set; }
        public int InsuredId { get; set; }
        public int? NoOfAccident { get; set; }
        public DateTime? RequestPolicyEffectiveDate { get; set; }
        public int? NajmNcdFreeYears { get; set; }
        public string CustomCardNumber { get; set; }
        public string SequenceNumber { get; set; }
        public string NationalId { get; set; }
        public string DriverNIN { get; set; }

        public int? VehicleIdTypeId { get; set; }
        public string VehicleMaker { get; set; }
        public short? VehicleMakerCode { get; set; }
        public string VehicleModel { get; set; }
        public short? ModelYear { get; set; }
        public byte? PlateTypeCode { get; set; }
        public string CarPlateText1 { get; set; }
        public string CarPlateText2 { get; set; }
        public string CarPlateText3 { get; set; }
        public short? CarPlateNumber { get; set; }
        public int? VehicleValue { get; set; }
        public VehicleIdType VehicleIdType
        {
            get { return (VehicleIdType)VehicleIdTypeId; }
            set { VehicleIdTypeId = (int)value; }
        }

        public bool? VehicleAgencyRepair { get; set; }
        public int? DeductibleValue { get; set; }
        public short? InsuranceTypeCode { get; set; }
        public long QuotationResponseCityId { get; set; }
        public string ReferenceId { get; set; }
        public DateTime QuotationResponseCreatedDate { get; set; }

        public Guid DriverId { get; set; }
        public string DriverFirstName { get; set; }
        public string DriverSecondName { get; set; }
        public string DriverLastName { get; set; }
        public string DriverEnglishFirstName { get; set; }
        public string DriverEnglishSecondName { get; set; }
        public string DriverEnglishLastName { get; set; }

        public string InsuredFirstNameAr { get; set; }
        public long? InsuredCityId { get; set; }
        public string CityArabicDescription { get; set; }
        public string CityEnglishDescription { get; set; }

        public long? CityYakeenCode { get; set; }
        public int InsuranceCompanyID { get; set; }
        public string CompanyKey { get; set; }
        public string CompanyNameAR { get; set; }
        public string CompanyNameEn { get; set; }

        public string AddressCity { get; set; }
        public int? AddressId { get; set; }

        public string ExternalId { get; set; }

        public List<DriverData> AdditionalDriverList { get; set; }
        public string EducationLevelAr { get; set; }
        public string EducationLevelEn { get; set; }
        public int? MileageExpectedAnnualId { get; set; }
        public int? ParkingLocationId { get; set; }
        public bool ActiveTabbyTPL { get; set; }        public bool ActiveTabbyComp { get; set; }
        public bool ActiveTabbySanadPlus { get; set; }        public bool ActiveTabbyWafiSmart { get; set; }
        public bool ActiveTabbyMotorPlus { get; set; }
        public bool? IsRenewal { get; set; }
    }
}
