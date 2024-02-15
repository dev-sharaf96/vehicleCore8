using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto.Providers
{
    public class QuotationServiceRequest
    {
        public string ReferenceId { get; set; }

        [JsonIgnore]
        public string QuotationRequestExternalId { get; set; }
        [JsonIgnore]
        public int InsuranceCompanyCode { get; set; }

        [JsonIgnore]
        public bool? IsUseNumberOfAccident { get; set; }
        /// <summary>
        /// save value of Insured Birth date hijri 
        /// this property for medGulf insurance Company Only
        /// it want to set InsuredBirthDate Gerean in all Cases .
        /// </summary>
        [JsonIgnore]
        public string InsuredBirthDateH { get; set; }

        /// <summary>
        /// save value of Insured Birth date Gorgean 
        /// this property for medGulf insurance Company Only
        /// it want to set InsuredBirthDate Gerean in all Cases .
        /// </summary>
        [JsonIgnore]
        public string InsuredBirthDateG { get; set; }



        public int ProductTypeCode { get; set; }
        public DateTime PolicyEffectiveDate { get; set; }
        public int InsuredIdTypeCode { get; set; }
        public long InsuredId { get; set; }
        public string InsuredBirthDate { get; set; }
        public string InsuredGenderCode { get; set; }
        public string InsuredNationalityCode { get; set; }
        public string InsuredIdIssuePlaceCode { get; set; }
        public string InsuredIdIssuePlace { get; set; }
        public string InsuredCityCode { get; set; }
        public string InsuredCity { get; set; }
        public int VehicleIdTypeCode { get; set; }
        public long VehicleId { get; set; }
        public bool VehicleOwnerTransfer { get; set; } 
        public long VehicleOwnerId { get; set; }
        public string VehiclePlateTypeCode { get; set; }
        public int VehicleModelYear { get; set; }
        public string VehicleMaker { get; set; }
        public string VehicleMakerCode { get; set; }
        public string VehicleModel { get; set; }
        public string VehicleModelCode { get; set; }
        public string VehicleMajorColor { get; set; }
        public string VehicleMajorColorCode { get; set; }
        public string VehicleBodyTypeCode { get; set; }
        public string VehicleRegPlaceCode { get; set; }
        public string VehicleRegPlace { get; set; }
        public string VehicleRegExpiryDate { get; set; }

         [JsonIgnore]
        public int VehicleCapacity { get; set; }


        public int VehicleCylinders { get; set; }
        public int VehicleWeight { get; set; }
        public int VehicleLoad { get; set; }

        [JsonIgnore]
        public bool VehicleUsingWorkPurposes { get; set; }
        [JsonIgnore]
        public bool DriverDisabled { get; set; }

        [JsonIgnore]
        public bool IsCarRegistered { get; set; }
        public int NCDFreeYears { get; set; }
        public string NCDReference { get; set; }

        public int? VehicleValue { get; set; }
        public int? DeductibleValue { get; set; }
        public bool? VehicleAgencyRepair { get; set; }

        // Yakeen
        //public int CarPlateNumber { get; set; }
        //public string CarPlateText { get; set; }
        public string VehicleOwnerName { get; set; }
        public string InsuredFirstNameAr { get; set; }
        public string InsuredMiddleNameAr { get; set; }
        public string InsuredLastNameAr { get; set; }
        public string InsuredFirstNameEn { get; set; }
        public string InsuredMiddleNameEn { get; set; }
        public string InsuredLastNameEn { get; set; }
        public virtual List<DriverDto> Drivers { get; set; }

        // Yakeen - 4 properties used only for WAFA
        public string VehicleChassisNumber { get; set; }
        public int? VehiclePlateNumber { get; set; }
        public string VehiclePlateText1 { get; set; }
        public string VehiclePlateText2 { get; set; }
        public string VehiclePlateText3 { get; set; }

        // new fields need to be sent 
        public string InsuredSocialStatusCode { get; set; }
        public string PromoCode { get; set; }
        public int? InsuredEducationCode { get; set; }
        public string InsuredOccupationCode { get; set; }
        public string InsuredOccupation { get; set; }
        public int? InsuredChildrenBelow16Years { get; set; }
        public string InsuredWorkCityCode { get; set; }
        public string InsuredWorkCity { get; set; }
        public int VehicleUseCode { get; set; }
        public int? VehicleMileage { get; set; }
        public int? VehicleTransmissionTypeCode { get; set; }
        public int? VehicleMileageExpectedAnnualCode { get; set; }
        public int? VehicleAxleWeight { get; set; }
        public int? VehicleEngineSizeCode { get; set; }
        public int? VehicleOvernightParkingLocationCode { get; set; }
        public bool VehicleModification { get; set; }
        public string VehicleModificationDetails { get; set; }
        public int? VehicleAxleWeightCode { get; set; }
        [JsonProperty("NoOfAccident", NullValueHandling = NullValueHandling.Ignore)]
        public int? NoOfAccident { get; set; }
        [JsonProperty("Accidents", NullValueHandling = NullValueHandling.Ignore)]
        public List<Accident> Accidents { get; set; }
        public List<VehicleSpecificationDto> VehicleSpecifications { get; set; }

        [JsonProperty("PostCode", NullValueHandling = NullValueHandling.Ignore)]
        public string PostalCode { get; set; }
        [JsonProperty("ManualEntry", NullValueHandling = NullValueHandling.Ignore)]
        public string ManualEntry { get; set; }
        [JsonProperty("ReferenceNo", NullValueHandling = NullValueHandling.Ignore)]
        public string ReferenceNo { get; set; }

        [JsonProperty("MissingFields", NullValueHandling = NullValueHandling.Ignore)]
        public string MissingFields { get; set; }
        [JsonProperty("InsuredAddressRegionID", NullValueHandling = NullValueHandling.Ignore)]
        public int? InsuredAddressRegionID { get; set; }
        [JsonProperty("IdExpiryDate", NullValueHandling = NullValueHandling.Ignore)]
        public string IdExpiryDate { get; set; }
        [JsonProperty("CameraTypeId", NullValueHandling = NullValueHandling.Ignore)]
        public int? CameraTypeId { get; set; }
        [JsonProperty("BrakeSystemId", NullValueHandling = NullValueHandling.Ignore)]
        public int? BrakeSystemId { get; set; }
        [JsonProperty("HasAntiTheftAlarm", NullValueHandling = NullValueHandling.Ignore)]
        public bool? HasAntiTheftAlarm { get; set; }
        [JsonProperty("ParkingSensorId", NullValueHandling = NullValueHandling.Ignore)]
        public int? ParkingSensorId { get; set; }
        [JsonProperty("IsRenewal", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsRenewal { get; set; }
        [JsonProperty("IsUser", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsUser { get; set; }
        [JsonProperty("HasFireExtinguisher", NullValueHandling = NullValueHandling.Ignore)]
        public bool? HasFireExtinguisher { get; set; }
        [JsonProperty("MobileNo", NullValueHandling = NullValueHandling.Ignore)]
        public string MobileNo { get; set; }

        [JsonProperty("QuotationNo", NullValueHandling = NullValueHandling.Ignore)]
        public string QuotationNo { get; set; }

        [JsonIgnore]
        public int? BankId { get; set; }

        [JsonProperty("LessorBuildingNo", NullValueHandling = NullValueHandling.Ignore)]
        public int? LessorBuildingNo { get; set; }

        [JsonProperty("LessorZipCode", NullValueHandling = NullValueHandling.Ignore)]
        public int? LessorZipCode { get; set; }

        [JsonProperty("LessorAdditionalNumber", NullValueHandling = NullValueHandling.Ignore)]
        public int? LessorAdditionalNumber { get; set; }

        [JsonProperty("LessorCity", NullValueHandling = NullValueHandling.Ignore)]
        public string LessorCity { get; set; }

        [JsonProperty("LessorDistrict", NullValueHandling = NullValueHandling.Ignore)]
        public string LessorDistrict { get; set; }

        [JsonProperty("LessorStreet", NullValueHandling = NullValueHandling.Ignore)]
        public string LessorStreet { get; set; }

        [JsonIgnore]
        public string WataniyaVehicleMakerCode { get; set; }
        [JsonIgnore]
        public string WataniyaVehicleModelCode { get; set; }
        [JsonIgnore]
        public string Street { get; set; }
        [JsonIgnore]
        public string District { get; set; }
        [JsonIgnore]
        public string City { get; set; }
        [JsonIgnore]
        public int? BuildingNumber { get; set; }
        [JsonIgnore]
        public int? ZipCode { get; set; }
        [JsonIgnore]
        public int? AdditionalNumber { get; set; }
        [JsonIgnore]
        public int? WataniyaFirstPlateLetterID { get; set; }
        [JsonIgnore]
        public int? WataniyaSecondPlateLetterID { get; set; }
        [JsonIgnore]
        public int? WataniyaThirdPlateLetterID { get; set; }

        [JsonProperty("PolicyNo", NullValueHandling = NullValueHandling.Ignore)]
        public string PolicyNo { get; set; }

        [JsonProperty("PolicyExpiryDate", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? PolicyExpiryDate { get; set; }

        [JsonProperty("HasTrailer", NullValueHandling = NullValueHandling.Ignore)]
        public bool? HasTrailer { get; set; }

        [JsonProperty("TrailerSumInsured", NullValueHandling = NullValueHandling.Ignore)]
        public int? TrailerSumInsured { get; set; }

        [JsonProperty("OtherUses", NullValueHandling = NullValueHandling.Ignore)]
        public bool? OtherUses { get; set; }
    }

    public class QuotationServiceRequestWithOutAccident
    {
        public QuotationServiceRequestWithOutAccident()
        {

        }

        public string ReferenceId { get; set; }
        [JsonIgnore]
        public string QuotationRequestExternalId { get; set; }

        [JsonIgnore]
        public int InsuranceCompanyCode { get; set; }

        [JsonIgnore]
        public bool? IsUseNumberOfAccident { get; set; }

        /// <summary>
        /// save value of Insured Birth date hijri 
        /// this property for medGulf insurance Company Only
        /// it want to set InsuredBirthDate Gerean in all Cases .
        /// </summary>
        [JsonIgnore]
        public string InsuredBirthDateH { get; set; }

        /// <summary>
        /// save value of Insured Birth date Gorgean 
        /// this property for medGulf insurance Company Only
        /// it want to set InsuredBirthDate Gerean in all Cases .
        /// </summary>
        [JsonIgnore]
        public string InsuredBirthDateG { get; set; }



        public int ProductTypeCode { get; set; }
        public DateTime PolicyEffectiveDate { get; set; }
        public int InsuredIdTypeCode { get; set; }
        public long InsuredId { get; set; }
        public string InsuredBirthDate { get; set; }
        public string InsuredGenderCode { get; set; }
        public string InsuredNationalityCode { get; set; }
        public string InsuredIdIssuePlaceCode { get; set; }
        public string InsuredIdIssuePlace { get; set; }
        public string InsuredCityCode { get; set; }
        public string InsuredCity { get; set; }
        public int VehicleIdTypeCode { get; set; }
        public long VehicleId { get; set; }
        public bool VehicleOwnerTransfer { get; set; }
        public long VehicleOwnerId { get; set; }
        public string VehiclePlateTypeCode { get; set; }
        public int VehicleModelYear { get; set; }
        public string VehicleMaker { get; set; }
        public string VehicleMakerCode { get; set; }
        public string VehicleModel { get; set; }
        public string VehicleModelCode { get; set; }
        public string VehicleMajorColor { get; set; }
        public string VehicleMajorColorCode { get; set; }
        public string VehicleBodyTypeCode { get; set; }
        public string VehicleRegPlaceCode { get; set; }
        public string VehicleRegPlace { get; set; }
        public string VehicleRegExpiryDate { get; set; }

        [JsonIgnore]
        public int VehicleCapacity { get; set; }


        public int VehicleCylinders { get; set; }
        public int VehicleWeight { get; set; }
        public int VehicleLoad { get; set; }

        [JsonIgnore]
        public bool VehicleUsingWorkPurposes { get; set; }
        [JsonIgnore]
        public bool DriverDisabled { get; set; }

        [JsonIgnore]
        public bool IsCarRegistered { get; set; }
        public int NCDFreeYears { get; set; }
        public string NCDReference { get; set; }

        public int? VehicleValue { get; set; }
        public int? DeductibleValue { get; set; }
        public bool? VehicleAgencyRepair { get; set; }

        // Yakeen
        //public int CarPlateNumber { get; set; }
        //public string CarPlateText { get; set; }
        public string VehicleOwnerName { get; set; }
        public string InsuredFirstNameAr { get; set; }
        public string InsuredMiddleNameAr { get; set; }
        public string InsuredLastNameAr { get; set; }
        public string InsuredFirstNameEn { get; set; }
        public string InsuredMiddleNameEn { get; set; }
        public string InsuredLastNameEn { get; set; }
        public virtual List<DriverDto> Drivers { get; set; }

        // Yakeen - 4 properties used only for WAFA
        public string VehicleChassisNumber { get; set; }
        public int VehiclePlateNumber { get; set; }
        public string VehiclePlateText1 { get; set; }
        public string VehiclePlateText2 { get; set; }
        public string VehiclePlateText3 { get; set; }

        // new fields need to be sent 
        public string InsuredSocialStatusCode { get; set; }
        public string PromoCode { get; set; }
        public int? InsuredEducationCode { get; set; }
        public string InsuredOccupationCode { get; set; }
        public string InsuredOccupation { get; set; }
        public int? InsuredChildrenBelow16Years { get; set; }
        public string InsuredWorkCityCode { get; set; }
        public string InsuredWorkCity { get; set; }
        public int VehicleUseCode { get; set; }
        public int? VehicleMileage { get; set; }
        public int? VehicleTransmissionTypeCode { get; set; }
        public int? VehicleMileageExpectedAnnualCode { get; set; }
        public int? VehicleAxleWeight { get; set; }
        public int? VehicleEngineSizeCode { get; set; }
        public int? VehicleOvernightParkingLocationCode { get; set; }
        public bool VehicleModification { get; set; }
        public string VehicleModificationDetails { get; set; }
        public int? VehicleAxleWeightCode { get; set; }
        public List<VehicleSpecificationDto> VehicleSpecifications { get; set; }



    }
    public class Accident
    {
        public string CaseNumber { get; set; }
        public DateTime AccidentDate { get; set; }
        public int Liability { get; set; }
        public string CityName { get; set; }
        public int DriverAge { get; set; }
        public string CarType { get; set; }
        public string DriverID { get; set; }
        public string SequenceNumber { get; set; }
        public string OwnerID { get; set; }
        public List<object> EstimatedAmount { get; set; }
        public string DamageParts { get; set; }
        public List<object> CarModel { get; set; }
        public List<object> CauseOfAccident { get; set; }
    }

}
