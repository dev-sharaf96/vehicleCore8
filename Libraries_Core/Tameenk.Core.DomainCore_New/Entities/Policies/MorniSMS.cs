using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
    public class MorniSMS : BaseEntity
    {
        public int ID { get; set; }
        public string PolicyNo { get; set; }
        public DateTime? PolicyStartDate { get; set; }
        public DateTime? PolicyEndDate { get; set; }
        public string vin { get; set; }
        public double VehicleRegistrationSerialNumber { get; set; }
        public string OwnerFirstName { get; set; }
        public string OwnerLastName { get; set; }
        public double OwnerMobilePhone { get; set; }
        public string PlateInfo { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public double ModelYear { get; set; }
        public bool IsSMSSent { get; set; }
        public int? Lang { get; set; }
        
    }
}
