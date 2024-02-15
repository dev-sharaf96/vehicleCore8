using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TameenkDAL.Models;

namespace Tameenk.Models
{
    public class MyPoliciesViewModel
    {
        public List<PolicyModel> PoliciesList { get; internal set; }
        public int PoliciesTotalCount { get; internal set; }
        public int CurrentPage { get; set; }
        public string Lang { get; set; }
    }
}