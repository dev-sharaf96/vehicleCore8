using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Policies
{
    /// <summary>
    /// Class of najm statistics
    /// </summary>
    public class NajmStatistics
    {
        /// <summary>
        /// Get or set the count of submited polices.
        /// </summary>
        public int Submited { get; set; }

        /// <summary>
        /// Get or set the count of rejected policies.
        /// </summary>
        public int Rejected { get; set; }

        /// <summary>
        /// Get or set the count of pending policies.
        /// </summary>
        public int Pending { get; set; }
    }
}
