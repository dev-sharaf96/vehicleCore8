using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation
{
    public class VehiclePolicyInfoData
    {
        public string PolicyNo { get; set; }
        public string ReferenceId { get; set; }
        public string CustomCardNumber { get; set; }
        public Int16 ModelYear { get; set; }
        public int InsuranceCompanyId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public Int16 SelectedInsuranceTypeCode { get; set; }
    }
}
