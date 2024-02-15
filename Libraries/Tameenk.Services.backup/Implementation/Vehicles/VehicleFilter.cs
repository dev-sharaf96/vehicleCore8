using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Vehicles
{
    /// <summary>
    /// Vehicle Filter
    /// </summary>
  public class VehicleFilter
    {
        /// <summary>
        /// Chassis Number
        /// </summary>
        public string ChassisNumber { get; set; }


        /// <summary>
        /// Sequence Number
        /// </summary>
        public string SequenceNumber { get; set; }

        /// <summary>
        /// Custom Card Number
        /// </summary>
        public string CustomCardNumber { get; set; }
        /// <summary>
        /// Vehicle Model Code
        /// </summary>
        public long? VehicleModelCode { get; set; }

       

        /// <summary>
        /// Vehicle Maker Code
        /// </summary>
        public short? VehicleMakerCode { get; set; }


        /// <summary>
        /// Car Plate Number.
        /// </summary>
        public Nullable<short> CarPlateNumber { get; set; }
    }
}
