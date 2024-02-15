using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Policies
{
    public class PolicyDataModel
    {
        public string PolicyNo { set; get; }
        public string ReferenceId { set; get; }
        public int InsuranceCompanyId { set; get; }
        public short InsuranceTypeCode { set; get; }
    }
}
