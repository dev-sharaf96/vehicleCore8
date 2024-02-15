using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Policies
{
    public class AutoleasingBankStatisticsListing
    {
        public int TotalPolicies { get; set; }
        public int TotalUnUploadedPolicies { get; set; }
        public int TotalCustomCard { get; set; }
        public int TotalSequence { get; set; }
    }
}
