using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Loggin.DAL
{
    /// <summary>
    /// Service Request filter
    /// </summary>
    
    public class ServiceRequestFilter
    {
        /// <summary>
        /// vehicle id represent ( sequence No / Custom No)
        /// </summary>
        public string VehicleId { get; set; }


        /// <summary>
        /// National Id
        /// </summary>
        public string NationalId { get; set; }

        /// <summary>
        /// Reference Id
        /// </summary>
        public string ReferenceNo { get; set; }


        /// <summary>
        /// Method
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Status Code
        /// </summary>
        public int? StatusCode { get; set; }


        /// <summary>
        /// insurance Company Id
        /// </summary>
        public int? InsuranceCompanyId { get; set; }


        /// <summary>
        /// Start date
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// End date
        /// </summary>
        public DateTime? EndDate { get; set; }

        public string PolicyNo { get; set; }
        public string InsuranceTypeId { get; set; }
    }
}
