using System;

namespace Tameenk.Services.Profile.Component.Models
{
    public class MyVehiclesDB
    {
        public System.Guid ID { get; set; }
        public short? VehicleMakerCode { get; set; }
        public string VehicleMaker { get; set; }
        public short? ModelYear { get; set; }
        public string VehicleModel { get; set; }
        public long? VehicleModelCode { get; set; }
        public string RegisterationPlace { get; set; }

        #region Plate
        public byte? PlateTypeCode { get; set; }
        public short? CarPlateNumber { get; set; }
        public string CarPlateText1 { get; set; }
        public string CarPlateText2 { get; set; }
        public string CarPlateText3 { get; set; }
        #endregion
    }
}