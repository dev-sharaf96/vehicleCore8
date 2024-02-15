using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TameenkDAL.Models
{
    public class VehicleModel
    {
        #region Vehicle Master Data.

        public System.Guid ID { get; set; }
        public string SequenceNumber { get; set; }
        public string CustomCardNumber { get; set; }
        public bool IsRegistered { get; set; }
        public Nullable<byte> Cylinders { get; set; }
        public string LicenseExpiryDate { get; set; }
        public string MajorColor { get; set; }
        public string MinorColor { get; set; }
        public Nullable<short> ModelYear { get; set; }
        public byte? PlateTypeCode { get; set; }
        public string RegisterationPlace { get; set; }
        public byte VehicleBodyCode { get; set; }
        public int VehicleWeight { get; set; }
        public int VehicleLoad { get; set; }
        public string VehicleMaker { get; set; }
        public string Vehicle_Model { get; set; }
        public string ChassisNumber { get; set; }
        public short? VehicleMakerCode { get; set; }
        public string VehicleModelCode { get; set; }

        [JsonProperty("carImage")]
        public string CarImage { get; set; }

        #endregion

        #region Another Objects Data
        public VehiclePlateModel VehiclePlate { get; set; }

        #endregion
    }
}
