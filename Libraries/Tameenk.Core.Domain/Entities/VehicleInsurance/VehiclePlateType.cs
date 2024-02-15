using System.Collections.Generic;

namespace Tameenk.Core.Domain.Entities.VehicleInsurance
{
    public class VehiclePlateType : BaseEntity
    {
        public VehiclePlateType()
        {
            Vehicles = new HashSet<Vehicle>();
        }
        
        public byte Code { get; set; }
        
        public string EnglishDescription { get; set; }
        
        public string ArabicDescription { get; set; }

        public ICollection<Vehicle> Vehicles { get; set; }
    }
}
