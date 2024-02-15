using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Policies
{
    public class TotalUsersPurchasedPoliciesModel
    {
        public string Item { get; set; }
        public int TotalIndividualUsersPurchasedPoliciesInterval { get; set; }
        public int TotalCorporateUsersPurchasedPoliciesInterval { get; set; }
    }
}
