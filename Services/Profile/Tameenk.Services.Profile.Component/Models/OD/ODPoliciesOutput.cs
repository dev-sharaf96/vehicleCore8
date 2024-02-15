using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Profile.Component.Models
{
    public class ODPoliciesOutput
    {
        public List<ODPolicyViewModel> PoliciesList { get; set; }
        public int PoliciesTotalCount { get; set; }
    }
}
