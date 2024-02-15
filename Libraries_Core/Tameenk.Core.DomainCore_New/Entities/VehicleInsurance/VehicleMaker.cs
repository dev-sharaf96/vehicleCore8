using System.Collections.Generic;

namespace Tameenk.Core.Domain.Entities.VehicleInsurance
{
    public class VehicleMaker : BaseEntity
    {
        public VehicleMaker()
        {
            VehicleModels = new HashSet<VehicleModel>();
        }
        
        public short Code { get; set; }
        
        public string EnglishDescription { get; set; }
        
        public string ArabicDescription { get; set; }

        public ICollection<VehicleModel> VehicleModels { get; set; }
    }
}
