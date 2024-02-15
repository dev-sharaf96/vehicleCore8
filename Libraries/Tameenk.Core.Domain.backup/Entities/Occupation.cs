using System.Collections.Generic;
using Tameenk.Core.Domain.Entities.Quotations;
using Tameenk.Core.Domain.Entities.VehicleInsurance;

namespace Tameenk.Core.Domain.Entities
{
    public class Occupation : BaseEntity
    {
        public Occupation()
        {
            Insureds = new List<Insured>();
            Drivers = new List<Driver>();
        }
        public int ID { get; set; }
        public string Code { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public bool? IsCitizen { get; set; }
        public bool? IsMale { get; set; }

        public virtual ICollection<Insured> Insureds { get; set; }
        public virtual ICollection<Driver> Drivers { get; set; }
    }
}
