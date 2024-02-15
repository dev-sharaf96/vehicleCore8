using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Loggin.DAL
{
    /// <summary>
    /// Checkout Request filter
    /// </summary>

    public class RequestLogFilter
    {
        /// <summary>
        /// vehicle id represent ( sequence No / Custom No)
        /// </summary>
        public string VehicleId { get; set; }


        /// <summary>
        /// National Id
        /// </summary>
        public string NIN { get; set; }

        /// <summary>
        /// Reference Id
        /// </summary>
        public string ReferenceId { get; set; }

        /// <summary>
        /// External Id
        /// </summary>
        public string ExternalId { get; set; }

        public DateTime? EndDate { get; internal set; }
        public DateTime? StartDate { get; internal set; }

        public int? Channel { get; set; }
        public string MethodName { get; set; }
    }
}
