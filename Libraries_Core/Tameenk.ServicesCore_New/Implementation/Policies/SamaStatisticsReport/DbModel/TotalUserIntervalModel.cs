using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Policies
{
    public class TotalUserIntervalModel
    {
        public string Item { get; set; }
        public int TotalIndividualUsersInterval { get; set; }
        public int TotalCorporateUsersInterval { get; set; }
    }
}
