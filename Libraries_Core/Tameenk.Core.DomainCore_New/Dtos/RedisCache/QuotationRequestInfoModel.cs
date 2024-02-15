using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.VehicleInsurance;
using Tameenk.Core.Domain.Enums.Quotations;
using Tameenk.Core.Domain.Enums.Vehicles;

namespace Tameenk.Core.Domain
{
    public class QuotationRequestInfoModel
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("externalId")]
        public string ExternalId { get; set; }

        [JsonProperty("mainDriverId")]
        public Guid MainDriverId { get; set; }

        [JsonProperty("vehicleId")]
        public Guid VehicleId { get; set; }

        [JsonProperty("cityCode")]
        public long CityCode { get; set; }

        [JsonProperty("requestPolicyEffectiveDate")]
        public DateTime? RequestPolicyEffectiveDate { get; set; }

        [JsonProperty("isRenewal")]
        public bool? IsRenewal { get; set; }

        [JsonProperty("noOfAccident")]
        public int? NoOfAccident { get; set; }

        [JsonProperty("najmResponse")]
        public string NajmResponse { get; set; }

        [JsonProperty("najmNcdFreeYears")]
        public int? NajmNcdFreeYears { get; set; }

        [JsonProperty("najmNcdRefrence")]
        public string NajmNcdRefrence { get; set; }

        [JsonProperty("postCode")]
        public string PostCode { get; set; }

        [JsonProperty("missingFields")]
        public string MissingFields { get; set; }

        [JsonProperty("insuredId")]
        public int InsuredId { get; set; }

        [JsonProperty("nationalId")]
        public string NationalId { get; set; }

        [JsonProperty("insuredCityYakeenCode")]
        public long InsuredCityYakeenCode { get; set; }

        [JsonProperty("cardIdTypeId")]
        public int CardIdTypeId { get; set; }

        [JsonProperty("insuredCityArabicDescription")]
        public string InsuredCityArabicDescription { get; set; }

        [JsonProperty("insuredBirthDate")]
        public DateTime InsuredBirthDate { get; set; }

        [JsonProperty("insuredBirthDateH")]
        public string InsuredBirthDateH { get; set; }

        [JsonProperty("insuredGenderId")]
        public int InsuredGenderId { get; set; }

        [JsonProperty("nationalityCode")]
        public string NationalityCode { get; set; }

        [JsonProperty("insuredFirstNameAr")]
        public string InsuredFirstNameAr { get; set; }

        [JsonProperty("insuredMiddleNameAr")]
        public string InsuredMiddleNameAr { get; set; }

        [JsonProperty("insuredLastNameAr")]
        public string InsuredLastNameAr { get; set; }

        [JsonProperty("insuredFirstNameEn")]
        public string InsuredFirstNameEn { get; set; }

        [JsonProperty("insuredMiddleNameEn")]
        public string InsuredMiddleNameEn { get; set; }

        [JsonProperty("insuredLastNameEn")]
        public string InsuredLastNameEn { get; set; }

        [JsonProperty("socialStatusId")]
        public int? SocialStatusId { get; set; }

        [JsonProperty("insuredOccupationCode")]
        public string InsuredOccupationCode { get; set; }

        [JsonProperty("insuredOccupationNameAr")]
        public string InsuredOccupationNameAr { get; set; }

        [JsonProperty("insuredOccupationNameEn")]
        public string InsuredOccupationNameEn { get; set; }

        [JsonProperty("educationId")]
        public int EducationId { get; set; }

        [JsonProperty("childrenBelow16Years")]
        public int? ChildrenBelow16Years { get; set; }

        [JsonProperty("idIssueCityYakeenCode")]
        public long? IdIssueCityYakeenCode { get; set; }

        [JsonProperty("idIssueCityArabicDescription")]
        public string IdIssueCityArabicDescription { get; set; }

        [JsonProperty("idIssueCityEnglishDescription")]
        public string IdIssueCityEnglishDescription { get; set; }

        [JsonProperty("workCityId")]
        public long? WorkCityId { get; set; }

        [JsonProperty("mainDriverNin")]
        public string MainDriverNin { get; set; }

        [JsonProperty("isSpecialNeed")]
        public bool? IsSpecialNeed { get; set; }

        [JsonProperty("idExpiryDate")]
        public string IdExpiryDate { get; set; }

        [JsonProperty("nOALast5Years")]
        public int? NOALast5Years { get; set; }

        [JsonProperty("nOCLast5Years")]
        public int? NOCLast5Years { get; set; }

        [JsonProperty("mainDriverSocialStatusId")]
        public int? MainDriverSocialStatusId { get; set; }

        [JsonProperty("drivingPercentage")]
        public int? DrivingPercentage { get; set; }

        [JsonProperty("medicalConditionId")]
        public int? MedicalConditionId { get; set; }

        [JsonProperty("mainDriverViolation")]
        public List<DriverViolation> MainDriverViolation { get; set; }

        [JsonProperty("mainDriverLicenses")]
        public List<DriverLicense> MainDriverLicenses { get; set; }

        [JsonProperty("additionalDrivers")]
        public List<Driver> AdditionalDrivers { get; set; }

        [JsonProperty("majorColor")]
        public string MajorColor { get; set; }

        [JsonProperty("vehicleBodyCode")]
        public byte VehicleBodyCode { get; set; }

        [JsonProperty("cylinders")]
        public byte? Cylinders { get; set; }

        [JsonProperty("engineSizeId")]
        public int? EngineSizeId { get; set; }

        [JsonProperty("customCardNumber")]
        public string CustomCardNumber { get; set; }

        [JsonProperty("sequenceNumber")]
        public string SequenceNumber { get; set; }

        [JsonProperty("ownerTransfer")]
        public bool OwnerTransfer { get; set; }

        [JsonProperty("registerationPlace")]
        public string RegisterationPlace { get; set; }

        [JsonProperty("vehicleIdTypeId")]
        public int VehicleIdTypeId { get; set; }

        [JsonProperty("carPlateNumber")]
        public short? CarPlateNumber { get; set; }

        [JsonProperty("carPlateText1")]
        public string CarPlateText1 { get; set; }

        [JsonProperty("carPlateText2")]
        public string CarPlateText2 { get; set; }

        [JsonProperty("carPlateText3")]
        public string CarPlateText3 { get; set; }

        [JsonProperty("carOwnerNIN")]
        public string CarOwnerNIN { get; set; }

        [JsonProperty("carOwnerName")]
        public string CarOwnerName { get; set; }

        [JsonProperty("plateTypeCode")]
        public byte? PlateTypeCode { get; set; }

        [JsonProperty("licenseExpiryDate")]
        public string LicenseExpiryDate { get; set; }

        [JsonProperty("modelYear")]
        public short? ModelYear { get; set; }

        [JsonProperty("vehicleMakerCode")]
        public short? VehicleMakerCode { get; set; }

        [JsonProperty("vehicleMaker")]
        public string VehicleMaker { get; set; }

        [JsonProperty("vehicleModel")]
        public string VehicleModel { get; set; }

        [JsonProperty("vehicleModelCode")]
        public long? VehicleModelCode { get; set; }

        [JsonProperty("vehicleLoad")]
        public int VehicleLoad { get; set; }

        [JsonProperty("vehicleWeight")]
        public int VehicleWeight { get; set; }

        [JsonProperty("isUsedCommercially")]
        public bool? IsUsedCommercially { get; set; }

        [JsonProperty("vehicleValue")]
        public int? VehicleValue { get; set; }

        [JsonProperty("vehicleUseId")]
        public int VehicleUseId { get; set; }

        [JsonProperty("currentMileageKM")]
        public decimal? CurrentMileageKM { get; set; }

        [JsonProperty("transmissionTypeId")]
        public int? TransmissionTypeId { get; set; }

        [JsonProperty("mileageExpectedAnnualId")]
        public int? MileageExpectedAnnualId { get; set; }

        [JsonProperty("axleWeightId")]
        public int? AxleWeightId { get; set; }

        [JsonProperty("parkingLocationId")]
        public int? ParkingLocationId { get; set; }

        [JsonProperty("hasModifications")]
        public bool HasModifications { get; set; }

        [JsonProperty("modificationDetails")]
        public string ModificationDetails { get; set; }

        [JsonProperty("chassisNumber")]
        public string ChassisNumber { get; set; }

        [JsonProperty("manualEntry")]
        public bool? ManualEntry { get; set; }

        [JsonProperty("hasAntiTheftAlarm")]
        public bool? HasAntiTheftAlarm { get; set; }

        [JsonProperty("hasFireExtinguisher")]
        public bool? HasFireExtinguisher { get; set; }

        [JsonProperty("hasTrailer")]
        public bool HasTrailer { get; set; }

        [JsonProperty("trailerSumInsured")]
        public int TrailerSumInsured { get; set; }

        [JsonProperty("otherUses")]
        public bool OtherUses { get; set; }

        [JsonProperty("quotationCreatedDate")]
        public DateTime QuotationCreatedDate { get; set; }
    }
}
