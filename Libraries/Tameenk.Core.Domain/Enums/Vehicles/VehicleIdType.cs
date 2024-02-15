using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Resources.Vehicles;

namespace Tameenk.Core.Domain.Enums.Vehicles
{
    public enum VehicleIdType
    {
        /// <summary>
        /// Sequence number.
        /// </summary>
        [LocalizedName(typeof(VehicleIdTypeResource), "SequenceNumber")]
        SequenceNumber = 1,

        /// <summary>
        /// Customer card
        /// </summary>
        [LocalizedName(typeof(VehicleIdTypeResource), "CustomCard")]
        CustomCard = 2
    }
}
