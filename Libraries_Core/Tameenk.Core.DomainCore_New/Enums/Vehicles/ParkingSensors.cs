using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Resources.Vehicles;

namespace Tameenk.Core.Domain.Enums.Vehicles
{
    public enum ParkingSensors
    {
        [LocalizedName(typeof(ParkingSensorResource), "None")]
        None = 1,
        [LocalizedName(typeof(ParkingSensorResource), "RearSensors")]
        RearSensors = 2,
        [LocalizedName(typeof(ParkingSensorResource), "FrontSensors")]
        FrontSensors = 3
    }
}
