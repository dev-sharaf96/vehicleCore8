using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services
{
    public class PolicyInformation
    {
        public string PolicyNo { get; set; }
        public DateTime?PolicyEffectiveDate { get; set; }
        public DateTime? PolicyExpiryDate { get; set; }
        public string ChassisNumber { get; set; }
        public string VehicleId{ get; set; }
        public string ownerFirstName { get; set; }
        public string OwnerLastName { get; set; }
        public string OwnerMobilePhone { get; set; }
        public string PlateInfo { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public short? ModelYear { get; set; }

    }
}
