using System;

namespace Tameeenk.Models
{
    public class VehicleModel
    {
        public Guid Id { get; set; }
        public string Maker { get; set; }
        public short MakerCode { get; set; }
        public string Model { get; set; }
        public short? ModelYear { get; set; }
        public int PlateTypeCode { get; set; }
        public string PlateColor { get; set; }
        public CarPlateInfo CarPlate { get; set; }
    }
}
