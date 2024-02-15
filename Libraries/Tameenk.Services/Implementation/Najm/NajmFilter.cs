using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Najm
{
    public class NajmFilter
    {
        /// <summary>
        /// Policy Holder Nin
        /// </summary>
        public string PolicyHolderNin { get; set; }

        /// <summary>
        /// Vehicle Id
        /// </summary>
        public string VehicleId { get; set; }

        /// <summary>
        /// NCD Reference
        /// </summary>
        public string NCDReference { get; set; }

        /// <summary>
        /// NCD FreeYears
        /// </summary>
        public int NCDFreeYears { get; set; }

        /// <summary>
        /// Is Deleted
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
