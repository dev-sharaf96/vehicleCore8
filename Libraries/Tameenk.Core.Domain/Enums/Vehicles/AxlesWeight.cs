using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Resources.Vehicles;

namespace Tameenk.Core.Domain.Enums.Vehicles
{
    public enum AxlesWeight
    {
        /// <summary>
        /// Not available
        /// </summary>
        [LocalizedName(typeof(AxlesWeightResource), "NotAvailable")]
        NotAvailable = 0,
        /// <summary>
        /// Up Tp 20 Tons
        /// </summary>
        [LocalizedName(typeof(AxlesWeightResource), "UpTo20")]
        UpTo20 = 1,
        /// <summary>
        /// 20 - 30 Tons
        /// </summary>
        [LocalizedName(typeof(AxlesWeightResource), "Between20And30")]
        Between20And30 = 2,
        /// <summary>
        /// 30 - 40 Tons
        /// </summary>
        [LocalizedName(typeof(AxlesWeightResource), "NotAvailable")]
        Between30And40 = 3,
        /// <summary>
        /// Above 40 Tons
        /// </summary>
        [LocalizedName(typeof(AxlesWeightResource), "NotAvailable")]
        Above40 = 4,

    }
}
