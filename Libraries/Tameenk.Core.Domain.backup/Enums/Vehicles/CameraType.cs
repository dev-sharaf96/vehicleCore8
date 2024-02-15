using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Resources.Vehicles;

namespace Tameenk.Core.Domain.Enums.Vehicles
{
    public enum VehicleCameraType
    {
        [LocalizedName(typeof(CameraTypeResource), "None")]
        None = 1,
        [LocalizedName(typeof(CameraTypeResource), "RearCamera")]
        RearCamera = 2,
        [LocalizedName(typeof(CameraTypeResource), "FrontCamera")]
        FrontCamera = 3,
        [LocalizedName(typeof(CameraTypeResource), "FullCamera")]
        FullCamera = 3
    }
}
