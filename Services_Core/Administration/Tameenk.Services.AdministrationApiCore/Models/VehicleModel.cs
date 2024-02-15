using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Core.Domain.Enums.Vehicles;

namespace Tameenk.Services.AdministrationApi.Models
{
    /// <summary>
    /// Represent vehicle
    /// </summary>
    [JsonObject("vehicle")]
    public class VehicleModel
    {

        /// <summary>
        /// Chassis Number
        /// </summary>
        [JsonProperty("chassisNumber")]
        public string ChassisNumber { get; set; }

        /// <summary>
        /// vehicle Id
        /// </summary>
        [JsonProperty("id")]
        public string ID { get; set; }


        /// <summary>
        /// Sequence Number
        /// </summary>
        [JsonProperty("sequenceNumber")]
        public string SequenceNumber { get; set; }

        /// <summary>
        /// Custom Card Number
        /// </summary>
        [JsonProperty("customCardNumber")]
        public string CustomCardNumber { get; set; }
        /// <summary>
        /// Vehicle Model Code
        /// </summary>
        [JsonProperty("vehicleModelCode")]
        public long? VehicleModelCode { get; set; }


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
        public byte PlateTypeCode { get; set; }

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
        [JsonProperty("minorColor")]
        public string MinorColor { get; set; }

        /// <summary>
        /// Model Year
        /// </summary>
        [JsonProperty("modelYear")]
        public short? ModelYear { get; set; }

        /// <summary>
        /// Registeration Place
        /// </summary>
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
        /// Car Owner NIN
        /// </summary>
        [JsonProperty("carOwnerNIN")]
        public string CarOwnerNIN { get; set; }

        /// <summary>
        /// CarOwnerName
        /// </summary>
        [JsonProperty("carOwnerName")]
        public string CarOwnerName { get; set; }

        /// <summary>
        /// Vehicle Value
        /// </summary>
        [JsonProperty("vehicleValue")]
        public int? VehicleValue { get; set; }

        /// <summary>
        /// Is Used Commercially
        /// </summary>
        [JsonProperty("isUsedCommercially")]
        public bool? IsUsedCommercially { get; set; }

        /// <summary>
        /// Owner Transfer
        /// </summary>
        [JsonProperty("ownerTransfer")]
        public bool OwnerTransfer { get; set; }

        /// <summary>
        /// Engine Size Id
        /// </summary>
        [JsonProperty("engineSizeId")]
        public int? EngineSizeId { get; set; }

        /// <summary>
        /// Vehicle Use Id
        /// </summary>
        [JsonProperty("vehicleUseId")]
        public int VehicleUseId { get; set; }

        /// <summary>
        /// Current Mileage KM
        /// </summary>
        [JsonProperty("currentMileageKM")]
        public decimal? CurrentMileageKM { get; set; }


        /// <summary>
        /// Transmission Type Id
        /// </summary>
        [JsonProperty("transmissionTypeId")]
        public int? TransmissionTypeId { get; set; }

        /// <summary>
        /// Mileage Expected Annual Id
        /// </summary>
        [JsonProperty("mileageExpectedAnnualId")]
        public int? MileageExpectedAnnualId { get; set; }

        /// <summary>
        /// Axle Weight Id
        /// </summary>
        [JsonProperty("axleWeightId")]
        public int? AxleWeightId { get; set; }

        /// <summary>
        /// Parking Location Id
        /// </summary>
        [JsonProperty("parkingLocationId")]
        public int? ParkingLocationId { get; set; }


        /// <summary>
        /// Has Modifications
        /// </summary>
        [JsonProperty("hasModifications")]
        public bool HasModifications { get; set; }

        /// <summary>
        /// Modification Details
        /// </summary>
        [JsonProperty("modificationDetails")]
        public string ModificationDetails { get; set; }

        /// <summary>
        /// Vehichle identity type identifier.
        /// </summary>
        [JsonProperty("vehicleIdTypeId")]
        public int VehicleIdTypeId { get; set; }

        /// <summary>
        /// Vehicle break system type
        /// </summary>
        [JsonProperty("brakeSystemId")]
        public BrakingSystem? BrakeSystemId { get; set; }

        /// <summary>
        /// Vehicle Cruise Control type
        /// </summary>
        [JsonProperty("cruiseControlTypeId")]
        public CruiseControlType? CruiseControlTypeId { get; set; }

        /// <summary>
        /// Vehicle Parking Sensor type
        /// </summary>
        [JsonProperty("parkingSensorId")]
        public ParkingSensors? ParkingSensorId { get; set; }

        /// <summary>
        /// Vehicle Camera type
        /// </summary>
        [JsonProperty("cameraTypeId")]
        public Tameenk.Core.Domain.Enums.Vehicles.VehicleCameraType? CameraTypeId { get; set; }
    }
}