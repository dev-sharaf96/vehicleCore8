using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamkeen.bll.Services.Yakeen.Models
{
    public class CarInfoResult
    {
        public CarInfoResult()
        {
            Error = new YakeenError();
        }


        public bool Success { get; set; }
        public YakeenError Error { get; set; }

        /// <summary>
        /// true --> Registered , flase --> notRegistered
        /// </summary>
        public bool IsRegistered { get; set; }


        public short Cylinders { get; set; }

        public string LicenseExpiryDate { get; set; }

        public int LogId { get; set; }

        public string MajorColor { get; set; }

        public string MinorColor { get; set; }

        public short ModelYear { get; set; }

        public short? PlateTypeCode { get; set; }

        public string Regplace { get; set; }

        public short VehicleBodyCode { get; set; }

        public int VehicleWeight { get; set; }

        public int VehicleLoad { get; set; }

        public int VehicleMakerCode { get; set; }

        public int VehicleModelCode { get; set; }

        public string VehicleMaker { get; set; }

        public string VehicleModel { get; set; }

        public string ChassisNumber { get; set; }

        public Guid InternalIdentifier { get; set; }
    }
}
