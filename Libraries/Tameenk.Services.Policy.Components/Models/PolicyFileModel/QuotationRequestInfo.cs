using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Enums.Quotations;
using Tameenk.Core.Domain.Enums.Vehicles;

namespace Tameenk.Services.Policy.Components
{
    public class QuotationRequestInfo
    {
        #region Quotation Request
        public int QuotationRequestId { get; set; }
        public string ExternalId { get; set; }
        public Guid MainDriverId { get; set; }
        public long CityCode { get; set; }
        public DateTime? RequestPolicyEffectiveDate { get; set; }
        //public Guid VehicleId { get; set; }
        public string UserId { get; set; }
        public string NajmNcdRefrence { get; set; }
        public int? NajmNcdFreeYears { get; set; }
        public DateTime QuotationRequestCreatedDateTime { get; set; }
        //public bool IsComprehensiveGenerated { get; set; }
        //public bool IsComprehensiveRequested { get; set; }
        public int InsuredId { get; set; }
        public int? NoOfAccident { get; set; }
        public string NajmResponse { get; set; }
        //public bool? ManualEntry { get; set; }
        //public bool? IsRenewal { get; set; }
        public string PostCode { get; set; }
        //public string PreviousReferenceId { get; set; }
        //public string MissingFields { get; set; }
        //public Guid? AdditionalDriverIdOne { get; set; }
        //public Guid? AdditionalDriverIdTwo { get; set; }
        //public Guid? AdditionalDriverIdThree { get; set; }
        //public Guid? AdditionalDriverIdFour { get; set; }
        //public Guid? AutoleasingTransactionId { get; set; }
        //public bool? AutoleasingBulkOption { get; set; }
        //public bool? AutoleasingInitialOption { get; set; }
        //public int? AutoleasingContractDuration { get; set; }
        //public bool? IsConverted { get; set; }
        //public bool? ShowInitial { get; set; }
        //public string InitialExternalId { get; set; }
        #endregion

        #region Quotation Response
        public long QuotationResponseId { get; set; }
        //public bool CompanyAllowAnonymous { get; set; }
        //public bool AnonymousRequest { get; set; }
        //public bool HasDiscount { get; set; }
        //public string DiscountText { get; set; }
        //public int? RequestId { get; set; }
        public short? InsuranceTypeCode { get; set; }
        public DateTime QuotationResponseCreateDateTime { get; set; }
        public bool? VehicleAgencyRepair { get; set; }
        public int? DeductibleValue { get; set; }
        public string ReferenceId { get; set; }
        public string ICQuoteReferenceNo { get; set; }
        public int InsuranceCompanyId { get; set; }
        public string PromotionProgramCode { get; set; }
        public int PromotionProgramId { get; set; }
        //public long? CityId { get; set; }
        //public bool IsCheckedOut { get; set; }
        #endregion

        #region Insured
        public int CardIdTypeId { get; set; }
        public string NationalId { get; set; }
        public DateTime InsuredBirthDate { get; set; }
        public string InsuredBirthDateH { get; set; }
        public int InsuredGenderId { get; set; }
        public string InsuredNationalityCode { get; set; }
        public long? IdIssueCityId { get; set; }
        public string InsuredNameAr { get; set; }
        //public string MiddleNameAr { get; set; }
        //public string LastNameAr { get; set; }
        public string InsuredNameEn { get; set; }
        //public string MiddleNameEn { get; set; }
        //public string LastNameEn { get; set; }
        public int? SocialStatusId { get; set; }
        public int? OccupationId { get; set; }
        public string ResidentOccupation { get; set; }
        public int EducationId { get; set; }
        public int? ChildrenBelow16Years { get; set; }
        public long? WorkCityId { get; set; }
        public DateTime? InsuredCreatedDateTime { get; set; }
        public DateTime? InsuredModifiedDateTime { get; set; }
        public long? UserSelectedWorkCityId { get; set; }
        public long? UserSelectedCityId { get; set; }
        public int? AddressId { get; set; }
        public string OccupationName { get; set; }
        public string OccupationCode { get; set; }
        #endregion

        #region Vehicles
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
        public long? ColorCode { get; set; }
        //public bool? ManualEntry { get; set; }
        //public string MissingFields { get; set; }
        public int VehicleIdTypeId { get; set; }



        public ParkingLocation ParkingLocation
        {
            get { return (ParkingLocation)ParkingLocationId.GetValueOrDefault(); }
            set { ParkingLocationId = (int)ParkingLocation.RoadSide; }
        }

        public AxlesWeight? AxlesWeight
        {
            get { return (AxlesWeight)AxleWeightId.GetValueOrDefault(); }
            set { AxleWeightId = null; }
        }

        /// <summary>
        /// Expected mileage usage of this vehicle in kilometers.
        /// </summary>
        public Mileage? MileageExpectedAnnual
        {
            get { return (Mileage)MileageExpectedAnnualId.GetValueOrDefault(); }
            set { MileageExpectedAnnualId = null; }
        }

        /// <summary>
        /// Transmission type.
        /// </summary>
        public TransmissionType? TransmissionType
        {
            get { return (TransmissionType)TransmissionTypeId.GetValueOrDefault(); }
            set { TransmissionTypeId = null; }
        }
        /// <summary>
        /// Vehicle usage.
        /// </summary>
        public VehicleUse VehicleUse
        {
            get { return (VehicleUse)VehicleUseId; }
            //set { VehicleUseId = (int)VehicleUse.Private; }
        }
        /// <summary>
        /// The engine size.
        /// </summary>
        public EngineSize? EngineSize
        {
            get { return (EngineSize)EngineSizeId.GetValueOrDefault(); }
            set { EngineSizeId = null; }
        }

        /// <summary>
        /// The vehicle identity type.
        /// </summary>
        public VehicleIdType VehicleIdType
        {
            get { return (VehicleIdType)VehicleIdTypeId; }
            set { VehicleIdTypeId = (int)value; }
        }

        public bool HasTrailer { get; set; }
        public int TrailerSumInsured { get; set; }
        public bool OtherUses { get; set; }


        #endregion

        #region Driver
        public Guid DriverId { get; set; }

        public bool MainDriverIsCitizen { get; set; }

        public string MainDriverFullNameEn { get; set; }



        //public string EnglishThirdName { get; set; }

        public string MainDriverFullNameAr { get; set; }

        //public string SecondName { get; set; }

        //public string FirstName { get; set; }

        //public string ThirdName { get; set; }

        public string MainDriverSubtribeName { get; set; }

        public DateTime MainDriverCreatedDateTime { get; set; }
        public DateTime MainDriverDateOfBirthG { get; set; }

        public short? MainDriverNationalityCode { get; set; }

        public string MainDriverDateOfBirthH { get; set; }

        public string MainDriverNin { get; set; }

        public int MainDriverGenderId { get; set; }

        public int? MainDriverNCDFreeYears { get; set; }

        public string MainDriverNCDReference { get; set; }

        public bool? MainDriverIsSpecialNeed { get; set; }

        public string MainDriverIdIssuePlace { get; set; }

        public string MainDriverIdExpiryDate { get; set; }
    
        public int? MainDriverDrivingPercentage { get; set; }

        public int? MainDriverChildrenBelow16Years { get; set; }

        public int MainDriverEducationId { get; set; }

        public int? MainDriverSocialStatusId { get; set; }

        public int? MainDriverOccupationId { get; set; }

        public int? MedicalConditionId { get; set; }

        public string MainDriverResidentOccupation { get; set; }

        public long? MainDriverCityId { get; set; }

        public long? MainDriverWorkCityId { get; set; }

        public int? MainDriverNOALast5Years { get; set; }

        public int? MainDriverNOCLast5Years { get; set; }

        public string MainDriverCityName { get; set; }
        public int? MainDriverAddressId { get; set; }
        public string MainDriverWorkCityName { get; set; }
        public string MainDriverOccupationName { get; set; }
        public string MainDriverEducationName { get; set; }
        public string MainDriverSocialStatusName { get; set; }
        public string MainDriverPostCode { get; set; }
        public string MainDriverExtraLicenses { get; set; }
        public string MainDriverLicenses { get; set; }
        public string MainDriverViolations { get; set; }
        public int? MainDriverViolationId { get; set; }
        public int? MainDriverSaudiLicenseHeldYears { get; set; }
        public string MainDriverOccupationCode { get; set; }
        public int? MainDriverRelationShipId { get; set; }
        public string MainDriverMobileNumber { get; set; }
      
      

        public Gender MainDriverGender
        {
            get { return (Gender)MainDriverGenderId; }
            set { MainDriverGenderId = (int)value; }
        }
        public string MainDriverOccupationAr { get; set; }
        public string MainDriverOccupationEn { get; set; }

        #endregion

        #region VehicleBodyType
        public byte VehicleBodyTypeCode { get; set; }
        public string VehicleBodyTypeEnglishDescription { get; set; }
        public string VehicleBodyTypeArabicDescription { get; set; }
        //public bool IsActive { get; set; }
        //public string YakeenCode { get; set; }
        #endregion

        #region VehiclePlateType
        public byte? VehiclePlateTypeCode { get; set; }
        public string VehiclePlateTypeEnglishDescription { get; set; }
        public string VehiclePlateTypeArabicDescription { get; set; }
        #endregion

        #region ProductType 
        public string ProductTypeArabicDescription { get; set; }
        public string ProductTypeEnglishDescription { get; set; }
        #endregion
    }
}
