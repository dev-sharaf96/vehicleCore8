using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Policies
{
   public class TotalInSystemModel
    {
        public string Item { get; set; }
        public int TotalIndividualUsersInTheSystem { get; set; }
        public int TotalCorporateUsersInTheSystem { get; set; }
        public int Index { get; set; }
    }
}
