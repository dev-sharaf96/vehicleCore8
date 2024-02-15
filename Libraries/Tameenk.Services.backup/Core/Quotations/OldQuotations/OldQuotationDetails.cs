using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Enums.Vehicles;

namespace Tameenk.Services.Core.Quotations
{
    public class OldQuotationDetails
    {
        public int? RequestId { get; set; }

        public short? InsuranceTypeCode { get; set; }

        public DateTime CreateDateTime { get; set; }

        public bool? VehicleAgencyRepair { get; set; }

        public int? DeductibleValue { get; set; }

        public string ReferenceId { get; set; }
        public string ICQuoteReferenceNo { get; set; }

        public int InsuranceCompanyId { get; set; }
        public string PromotionProgramCode { get; set; }
        public int PromotionProgramId { get; set; }
        public long? CityId { get; set; }
        public bool IsCheckedOut { get; set; }
        public bool AutoleasingInitialOption { get; set; } = false;

        public int CardIdTypeId { get; set; }
        public string NationalId { get; set; }
        public DateTime BirthDate { get; set; }
        public string BirthDateH { get; set; }
        public int GenderId { get; set; }
        public string NationalityCode { get; set; }
        public long? IdIssueCityId { get; set; }

        public string FirstNameAr { get; set; }
        public string MiddleNameAr { get; set; }
        public string LastNameAr { get; set; }
        public string FirstNameEn { get; set; }
        public string MiddleNameEn { get; set; }
        public string LastNameEn { get; set; }
        public int? SocialStatusId { get; set; }
        public int? OccupationId { get; set; }
        public string ResidentOccupation { get; set; }
        public int EducationId { get; set; }

        public int? ChildrenBelow16Years { get; set; }

        public long? WorkCityId { get; set; }

        public DateTime? CreatedDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public long? UserSelectedWorkCityId { get; set; }

        public long? UserSelectedCityId { get; set; }

        public int? AddressId { get; set; }

        public int? IdIssueCity { get; set; }
        public string OccupationName { get; set; }
        public string OccupationCode { get; set; }
        public Guid DriverId { get; set; }

        public bool IsCitizen { get; set; }

        public string EnglishFirstName { get; set; }

        public string EnglishLastName { get; set; }

        public string EnglishSecondName { get; set; }

        public string EnglishThirdName { get; set; }

        public string LastName { get; set; }

        public string SecondName { get; set; }

        public string FirstName { get; set; }

        public string ThirdName { get; set; }

        public string SubtribeName { get; set; }

        public DateTime DateOfBirthG { get; set; }

        public string DateOfBirthH { get; set; }
        public string NIN { get; set; }
        public int? NCDFreeYears { get; set; }

        public string NCDReference { get; set; }

        public bool? IsSpecialNeed { get; set; }

        public string IdIssuePlace { get; set; }

        public string IdExpiryDate { get; set; }

        public int? DrivingPercentage { get; set; }
        public int? MedicalConditionId { get; set; }
        public int? NOALast5Years { get; set; }
        public int? NOCLast5Years { get; set; }
        public string CityName { get; set; }
        public string WorkCityName { get; set; }
        public string EducationName { get; set; }
        public string SocialStatusName { get; set; }
        public string PostCode { get; set; }
        public string ExtraLicenses { get; set; }
        public string Licenses { get; set; }
        public string Violations { get; set; }
        public int? SaudiLicenseHeldYears { get; set; }

        public string SequenceNumber { get; set; }

        public string CustomCardNumber { get; set; }

        public byte? Cylinders { get; set; }

        public string LicenseExpiryDate { get; set; }

        public string MajorColor { get; set; }

        public string MinorColor { get; set; }

        public short? ModelYear { get; set; }

        public byte? PlateTypeCode { get; set; }

        public string RegisterationPlace { get; set; }

        public byte VehicleBodyCode { get; set; }

        public int VehicleWeight { get; set; }

        public int VehicleLoad { get; set; }

        public string VehicleMaker { get; set; }

        public string VehicleModel { get; set; }

        public string ChassisNumber { get; set; }

        public short? VehicleMakerCode { get; set; }

        public long? VehicleModelCode { get; set; }

        public string CarPlateText1 { get; set; }

        public string CarPlateText2 { get; set; }

        public string CarPlateText3 { get; set; }

        public short? CarPlateNumber { get; set; }

        public string CarOwnerNIN { get; set; }

        public string CarOwnerName { get; set; }

        public int? VehicleValue { get; set; }

        public bool? IsUsedCommercially { get; set; }
        public bool OwnerTransfer { get; set; }

        public int? EngineSizeId { get; set; }
        public int VehicleUseId { get; set; }
        public decimal? CurrentMileageKM { get; set; }
        public int? TransmissionTypeId { get; set; }
        public int? MileageExpectedAnnualId { get; set; }
        public int? AxleWeightId { get; set; }
        public int? ParkingLocationId { get; set; }
        public bool HasModifications { get; set; }
        public bool? HasAntiTheftAlarm { get; set; }
        public bool? HasFireExtinguisher { get; set; }
        public string ModificationDetails { get; set; }
        public BrakingSystem? BrakeSystemId { get; set; }
        public CruiseControlType? CruiseControlTypeId { get; set; }
        public ParkingSensors? ParkingSensorId { get; set; }
        public long? ColorCode { get; set; }
        public bool? ManualEntry { get; set; }
        public string MissingFields { get; set; }
        public string CompanyName { get; set; }
        public string PriceDetail { get; set; }
        public string Product_benefit { get; set; }
        public string AdditionalDriverIdOne { get; set; }
        public int? VehicleLimitValue { get; set; }
    }
}
