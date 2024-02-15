using System;

namespace Tameenk.Core.Domain.Entities
{
    public class NajmResponseEntity : BaseEntity
    {
        public int Id { get; set; }
        public int IsVehicleRegistered { get; set; }
        public string PolicyHolderNin { get; set; }
        public string VehicleId { get; set; }
        public string NCDReference { get; set; }
        public int NCDFreeYears { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;
    }
}
