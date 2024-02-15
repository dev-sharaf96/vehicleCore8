using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamkeen.bll.Services.Yakeen.Models
{
    public class YakeenRequest
    {

        public string Token { get { return "yakeen_token"; } }
        public bool IsCitizen { get; set; }
        public string ReferenceNumber { get; set; }

        public long NIN { get; set; }
        /// <summary>
        /// format : (dd-MM-yyyy)
        /// </summary>
        /// 
        public string DateOfBirth { get; set; }

        public string LicenseExpiryDate { get; set; }

        public bool IsCarRegistered { get; set; }
        public long CarOwnerId { get; set; }
        public int CarSequenceNumber { get; set; }
        public short CarModelYear { get; set; }
        public long CustomCarCardNumber { get; set; }
        public bool IsVehicleUsedCommercially { get; set; }
        public int VehicleValue { get; set; }
        public bool IsDriverSpecialNeed { get; set; }
    }
}
