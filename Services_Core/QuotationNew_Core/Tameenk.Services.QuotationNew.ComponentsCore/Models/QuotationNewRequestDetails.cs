using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums.Quotations;
using Tameenk.Core.Domain.Enums.Vehicles;

namespace Tameenk.Services.QuotationNew.Components
{
    public class QuotationNewRequestDetails
    {
        /**** QuotationRequest *****/
        public int ID { get; set; }
        public string ExternalId { get; set; }
        public Guid MainDriverId { get; set; }
        public Guid VehicleId { get; set; }
        public long CityCode { get; set; }
        public DateTime? RequestPolicyEffectiveDate { get; set; }
        public bool? IsRenewal { get; set; }
        public int? NoOfAccident { get; set; }
        public string NajmResponse { get; set; }
        public int? NajmNcdFreeYears { get; set; }
        public string NajmNcdRefrence { get; set; }
        public string PostCode { get; set; }
        public string MissingFields { get; set; }
        public int InsuredId { get; set; }
        

        /**** Insured *****/
        public string NationalId { get; set; }
        public long InsuredCityYakeenCode { get; set; }
        public int CardIdTypeId { get; set; }
        public string InsuredCityArabicDescription { get; set; }
        public DateTime InsuredBirthDate { get; set; }
        public string InsuredBirthDateH { get; set; }

        public int InsuredGenderId { get; set; }
        public string NationalityCode { get; set; }
        public string InsuredFirstNameAr { get; set; }
        public string InsuredMiddleNameAr { get; set; }
        public string InsuredLastNameAr { get; set; }
        public string InsuredFirstNameEn { get; set; }
        public string InsuredMiddleNameEn { get; set; }
        public string InsuredLastNameEn { get; set; }
        public int? SocialStatusId { get; set; }
        public string InsuredOccupationCode { get; set; }
        public string InsuredOccupationNameAr { get; set; }
        public string InsuredOccupationNameEn { get; set; }
        public int EducationId { get; set; }
        public int? ChildrenBelow16Years { get; set; }
        public long? IdIssueCityYakeenCode { get; set; }
        public string IdIssueCityArabicDescription { get; set; }
        public string IdIssueCityEnglishDescription { get; set; }
        public long? WorkCityId { get; set; }
        public CardIdType CardIdType
        {
            get { return (CardIdType)CardIdTypeId; }
            set { CardIdTypeId = (int)value; }
        }
        public Gender InsuredGender
        {
            get { return (Gender)InsuredGenderId; }
            set { InsuredGenderId = (int)value; }
        }
        public SocialStatus? SocialStatus
        {
            get { return (SocialStatus)SocialStatusId; }
            set { SocialStatusId = null; }
        }
        public Education InsuredEducation
        {
            get { return (Education)EducationId; }
            set { EducationId = (int)Education.Academic; }
        }


        /**** MainDriver *****/
        public string MainDriverNin { get; set; }
        public bool? IsSpecialNeed { get; set; }
        public string IdExpiryDate { get; set; }
        public int? NOALast5Years { get; set; }
        public int? NOCLast5Years { get; set; }
        public int? MainDriverSocialStatusId { get; set; }
        public int? DrivingPercentage { get; set; }
        public int? MedicalConditionId { get; set; }
        public List<DriverViolation> MainDriverViolation { get; set; }
        public List<DriverLicense> MainDriverLicenses { get; set; }
        /****Additional Drivers****/
        public List<Driver> AdditionalDrivers { get; set; }

        /**** Vehicle *****/
        public string MajorColor { get; set; }
        public byte VehicleBodyCode { get; set; }
        public byte? Cylinders { get; set; }
        public int? EngineSizeId { get; set; }
        public string CustomCardNumber { get; set; }
        public string SequenceNumber { get; set; }
        public bool OwnerTransfer { get; set; }
        public string RegisterationPlace { get; set; }
        public int VehicleIdTypeId { get; set; }
        public short? CarPlateNumber { get; set; }
        public string CarPlateText1 { get; set; }
        public string CarPlateText2 { get; set; }
        public string CarPlateText3 { get; set; }
        public string CarOwnerNIN { get; set; }
        public string CarOwnerName { get; set; }
        public byte? PlateTypeCode { get; set; }
        public string LicenseExpiryDate { get; set; }
        public short? ModelYear { get; set; }
        public short? VehicleMakerCode { get; set; }
        public string VehicleMaker { get; set; }
        public string VehicleModel { get; set; }
        public long? VehicleModelCode { get; set; }
        public int VehicleLoad { get; set; }
        public int VehicleWeight { get; set; }
        public bool? IsUsedCommercially { get; set; }
      
        public int? VehicleValue { get; set; }
        public int VehicleUseId { get; set; }
        public decimal? CurrentMileageKM { get; set; }
        public int? TransmissionTypeId { get; set; }
        public int? MileageExpectedAnnualId { get; set; }
        public int? AxleWeightId { get; set; }
        public int? ParkingLocationId { get; set; }
        public bool HasModifications { get; set; }
        public string ModificationDetails { get; set; }
        public string ChassisNumber { get; set; }
        public bool? ManualEntry { get; set; }
       
        public bool? HasAntiTheftAlarm { get; set; }
        public bool? HasFireExtinguisher { get; set; }

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
        public DateTime QuotationCreatedDate { get; set; }
    }

}