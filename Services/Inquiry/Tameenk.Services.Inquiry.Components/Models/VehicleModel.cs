using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Tameenk.Services.Inquiry.Components
{
    /// <summary>
    /// Represent the vehicle model.
    /// </summary>
    [JsonObject("vehicle")]
    public class VehicleModel
    {

        /// <summary>
        /// Vehicle Maker Code
        /// </summary>
        [JsonProperty("vehicleMakerCode")]
        public short? VehicleMakerCode { get; set; }

        /// <summary>
        /// Vehicle Maker
        /// </summary>
        [JsonProperty("vehicleMaker")]
        public string VehicleMaker { get; set; }

        /// <summary>
        /// Vehcile Model
        /// </summary>
        [JsonProperty("model")]
        public string Model { get; set; }

        /// <summary>
        /// Vehicle Model Year
        /// </summary>
        [JsonProperty("vehicleModelYear")]
        public Nullable<short> VehicleModelYear { get; set; }

        /// <summary>
        /// Plate Color.
        /// </summary>
        [JsonProperty("plateColor")]
        public string PlateColor { get; set; }

        /// <summary>
        /// Car Plate Text1.
        /// </summary>
        [JsonProperty("carPlateText1")]
        public string CarPlateText1 { get; set; }

        /// <summary>
        /// Car Plate Text2.
        /// </summary>
        [JsonProperty("carPlateText2")]
        public string CarPlateText2 { get; set; }

        /// <summary>
        /// Car Plate Text3.
        /// </summary>
        [JsonProperty("carPlateText3")]
        public string CarPlateText3 { get; set; }

        /// <summary>
        /// Car Plate Number.
        /// </summary>
        [JsonProperty("carPlateNumber")]
        public Nullable<short> CarPlateNumber { get; set; }

        /// <summary>
        /// Car Plate Number Ar.
        /// </summary>
        [JsonProperty("carPlateNumberAr")]
        public string CarPlateNumberAr { get; set; }

        /// <summary>
        /// Car Plate Number En.
        /// </summary>
        [JsonProperty("carPlateNumberEn")]
        public string CarPlateNumberEn { get; set; }

        /// <summary>
        /// The Car Plate Text Ar.
        /// </summary>
        [JsonProperty("carPlateTextAr")]
        public string CarPlateTextAr { get; set; }

        /// <summary>
        /// The Car Plate Text En .
        /// </summary>
        [JsonProperty("carPlateTextEn")]
        public string CarPlateTextEn { get; set; }

        /// <summary>
        /// plate type code
        /// </summary>
        [JsonProperty("PlateTypeCode")]
        public byte? PlateTypeCode { get; set; }


        /// <summary>
        /// ID
        /// </summary>
        [JsonProperty("id")]
        public Guid ID { get; set; }

        /// <summary>
        /// Sequence Number
        /// </summary>
        [JsonProperty("sequenceNumber")]
        public string SequenceNumber { get; set; }

        /// <summary>
        /// custom Card Number
        /// </summary>
        [JsonProperty("customCardNumber")]
        public string CustomCardNumber { get; set; }

        //public bool IsRegistered { get; set; }

        /// <summary>
        /// Cylinders
        /// </summary>
        [JsonProperty("cylinders")]
        public byte? Cylinders { get; set; }

        /// <summary>
        /// License Expiry Date
        /// </summary>
        [JsonProperty("licenseExpiryDate")]
        public string LicenseExpiryDate { get; set; }

        /// <summary>
        /// Major Color
        /// </summary>
        [JsonProperty("majorColor")]
        public string MajorColor { get; set; }

        /// <summary>
        /// Minor Color
        /// </summary>
        [JsonProperty("MinorColor")]
        public string MinorColor { get; set; }

        /// <summary>
        /// Model Year
        /// </summary>
        [JsonProperty("modelYear")]
        public short? ModelYear { get; set; }


        /// <summary>
        /// Registeration Place
        /// </summary>
        [JsonProperty("registerationPlace")]
        public string RegisterationPlace { get; set; }

        /// <summary>
        /// Vehicle Body Code
        /// </summary>
        [JsonProperty("vehicleBodyCode")]
        public byte VehicleBodyCode { get; set; }

        /// <summary>
        /// Vehicle Weight
        /// </summary>
        [JsonProperty("vehicleWeight")]
        public int VehicleWeight { get; set; }

        /// <summary>
        /// Vehicle Load
        /// </summary>
        [JsonProperty("vehicleLoad")]
        public int VehicleLoad { get; set; }



        /// <summary>
        /// Chassis Number
        /// </summary>
        [JsonProperty("chassisNumber")]
        public string ChassisNumber { get; set; }


        /// <summary>
        /// Vehicle Model Code
        /// </summary>
        [JsonProperty("vehicleModelCode")]
        public long? VehicleModelCode { get; set; }

        /// <summary>
        /// The vehicle identifier.
        /// </summary>
        [JsonProperty("vehicleId")]
        [Required]
        public long VehicleId { get; set; }

        /// <summary>
        /// The estimated vehicle price.
        /// </summary>
        [JsonProperty("estimatedVehiclePrice")]
        [Display(Name = "estimatedVehiclePrice")]
        [Required(ErrorMessage = "inquiry.vehicle.vehicle_value_error_required")]
        [Range(10000, int.MaxValue, ErrorMessage = "inquiry.vehicle.vehicle_value_error_invalid")]
        public decimal ApproximateValue { get; set; }

        /// <summary>
        /// The manufacture year.
        /// </summary>
        [JsonProperty("manufactureYear")]
        public short? ManufactureYear { get; set; }

        /// <summary>
        /// The vehicle identity type identifier.
        /// </summary>
        [JsonProperty("VehicleIdTypeId")]
        public int VehicleIdTypeId { get; set; }

        /// <summary>
        /// The vehicle has modification.
        /// </summary>
        [JsonProperty("hasModification")]
        public bool HasModification { get; set; }

        /// <summary>
        /// The vehicle modification details.
        /// </summary>
        [JsonProperty("modification")]
        public string Modification { get; set; }

        /// <summary>
        /// The vehicle transmission type id.
        /// </summary>
        [JsonProperty("transmissionTypeId")]
        public int TransmissionTypeId { get; set; }

        /// <summary>
        /// The vehicle parking location identifier.
        /// </summary>
        [JsonProperty("parkingLocationId")]
        public int ParkingLocationId { get; set; }

        /// <summary>
        /// Is the vehicle owner trnasfer.
        /// </summary>
        [JsonProperty("ownerTransfer")]
        public bool OwnerTransfer { get; set; }

        /// <summary>
        /// Owner national identifier.
        /// </summary>
        [JsonProperty("ownerNationalId")]
        public string OwnerNationalId { get; set; }

        [JsonProperty("brakeSystemId")]
        public int? BrakeSystemId { get; set; }

        [JsonProperty("cruiseControlTypeId")]
        public int? CruiseControlTypeId { get; set; }

        [JsonProperty("parkingSensorId")]
        public int? ParkingSensorId { get; set; }

        [JsonProperty("cameraTypeId")]
        public int? CameraTypeId { get; set; }

        [JsonProperty("currentMileageKM")]
        public decimal? CurrentMileageKM { get; set; }

        [JsonProperty("hasAntiTheftAlarm")]
        public bool? HasAntiTheftAlarm { get; set; }

        [JsonProperty("hasFireExtinguisher")]
        public bool? HasFireExtinguisher { get; set; }

        [JsonProperty("carImage")]
        public string CarImage { get; set; }

        [JsonProperty("MileageExpectedAnnualId")]
        public int? MileageExpectedAnnualId { get; set; }

        /// <summary>
        /// Color Code
        /// </summary>
        [JsonProperty("colorCode")]
        public long? ColorCode { get; set; }

        /// <summary>
        /// for renewal only and it represent first vahivle value
        /// </summary>
        [JsonProperty("vehiclePrice")]
        public decimal vehiclePrice { get; set; }

        /// <summary>
        /// for renewal only and it represent policies No for this data (nin, vehicleId)
        /// </summary>
        [JsonProperty("policiesCount")]
        public int PoliciesCount { get; set; }

        /// <summary>
        /// for renewal only and it represent that custom catd converted to seq
        /// </summary>
        [JsonProperty("isCustomCardConverted")]
        public bool IsCustomCardConverted { get; set; }

        [JsonProperty("hasTrailer")]
        public bool HasTrailer { get; set; }

        [JsonProperty("trailerSumInsured")]
        public decimal ApproximateTrailerSumInsured { get; set; }

        [JsonProperty("otherUses")]
        public bool OtherUses { get; set; }
    }
}