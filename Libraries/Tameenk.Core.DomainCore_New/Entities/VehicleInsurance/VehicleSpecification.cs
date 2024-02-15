using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities.VehicleInsurance
{
    public class VehicleSpecification : BaseEntity
    {
        public VehicleSpecification()
        {
            Vehicles = new HashSet<Vehicle>();
        }
        /// <summary>
        /// Table id.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Code.
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// Arabic desccription.
        /// </summary>
        public string DescriptionAr { get; set; }
        /// <summary>
        /// English desccription.
        /// </summary>
        public string DescriptionEn { get; set; }

        public ICollection<Vehicle> Vehicles { get; set; }
    }
}
