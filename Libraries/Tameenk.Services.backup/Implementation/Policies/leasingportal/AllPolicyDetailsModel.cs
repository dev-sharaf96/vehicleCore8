using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Policies.leasingportal
{
   public class AllPolicyDetailsModel
    {
        public PolicyLogDetailsDTO PolicyDetails { get; set; }
        public LeasingPolicyPrice PolicyPricesDetails { get; set; }
    }
}
