using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto.Providers
{
    public class AdditionalInfoDetails
    {
        #region Breaking system
        public bool HasStandardBrakes { get; set; }
        public bool HasAntiLockBrakingSystem { get; set; }
        public bool HasAutomaticBrakingSystem { get; set; }
        #endregion
        #region Cruise control
        public bool HasCruiseControl { get; set; }
        public bool HasAdaptiveCruiseControl { get; set; }

        #endregion
        #region Sensor
        public bool HasRearParkingSensors { get; set; }
        public bool HasFrontSensors { get; set; }

        #endregion
        #region Camera
        public bool HasRearCamera { get; set; }
        public bool HasFrontCamera { get; set; }
        public bool Has360DegreeCamera { get; set; }
        #endregion
        #region ExpectedKMPerYear
        public string ExpectedKMPerYear { get; set; }
        #endregion
        #region Education Level
        public string EducationLevel { get; set; }
        #endregion
        #region SpentKM
        public string SpentKM { get; set; }
        #endregion
        #region Vehicle Transmission Type
        public string VehicleTransmissionType { get; set; }
        #endregion
        #region Vehicle Parking Place
        public string VehicleParkingPlace { get; set; }
        #endregion
        #region Vehicle Modification
        public string VehicleModifications { get; set; }
        #endregion
      
        public bool HasAntiThiefAlarm { get; set; }
        public bool HasFireExtinguisher { get; set; }

        #region ExtraDrivingLicence
        public string[] ExtraDrivingLicenceCountryAndYear { get; set; }
        #endregion
        #region Driver Medical Condition
        public string DriverMedicalCondition { get; set; }
        #endregion
        #region Occupation
        public string Occupation { get; set; }
        #endregion


        public int? VehicleEngineSizeCode { get; set; }
        public int VehicleUseCode { get; set; }
        public int VehicleTransmissionTypeCode { get; set; }
        public int? VehicleMileageExpectedAnnualCode { get; set; }
        public int VehicleAxleWeightCode { get; set; }
        public int? VehicleOvernightParkingLocationCode { get; set; }
        public bool VehicleModification { get; set; }
        public string VehicleModificationDetails { get; set; }
        public int InsuredEducationLevelCode { get; set; }
        public int InsuredChildrenCount { get; set; }
        public string InsuredHomecity { get; set; }
        public string InsuredWorkcity { get; set; }

    }
}
