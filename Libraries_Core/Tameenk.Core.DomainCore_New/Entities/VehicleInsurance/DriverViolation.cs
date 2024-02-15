using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities.VehicleInsurance
{
    public class DriverViolation : BaseEntity
    {
        /// <summary>
        /// Table id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Driver Id that has the violation
        /// </summary>
        public Guid DriverId { get; set; }
        /// <summary>
        /// Violation Id
        /// </summary>
        public int ViolationId { get; set; }
        public int? InsuredId { get; set; }
        public string NIN { get; set; }
        /// <summary>
        /// Driver navigation property
        /// </summary>
        public Driver Driver { get; set; }
    }
}
