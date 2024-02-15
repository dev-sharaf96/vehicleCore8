using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Services.Implementation.Policies;
using TameenkDAL.Models;

namespace Tameenk.Services.Profile.Component
{
    public class CorporateAccountPoliciesViewModel
    {
        public List<CorporatePolicyModel> PoliciesList { get; set; }
        public int PoliciesTotalCount { get; set; }
        public int CurrentPage { get; set; }
        public string Lang { get; set; }
    }
}
