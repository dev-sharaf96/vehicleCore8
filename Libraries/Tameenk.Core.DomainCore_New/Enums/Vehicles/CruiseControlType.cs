using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Resources.Vehicles;

namespace Tameenk.Core.Domain.Enums.Vehicles
{
    public enum CruiseControlType
    {
        [LocalizedName(typeof(CruiseControlTypeResource), "None")]
        None = 1,
        [LocalizedName(typeof(CruiseControlTypeResource), "CruiseControl")]
        CruiseControl = 2,
        [LocalizedName(typeof(CruiseControlTypeResource), "AdaptiveCruiseControl")]
        AdaptiveCruiseControl = 3
    }
}
