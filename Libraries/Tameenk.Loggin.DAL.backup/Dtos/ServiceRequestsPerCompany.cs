using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Loggin.DAL.Dtos
{
   public class ServiceRequestsPerCompany
    {
            /// <summary>
            /// Status Code
            /// </summary>
            public int? StatusCode { get; set; }

            /// <summary>
            /// createdOn
            /// </summary>
            public DateTime? StartDate { get; set; }

            public DateTime? EndDate { get; set; }
            
            public int? InsuranceTypeId { get; set; }

            public int? ModuleId { get; set; }
}
}
