using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Policies
{
   public class TotalcontPerAgeForAllRanges
    {
        public string range { get; set; }
        public int totalCountMale { get; set; }
        public int totalCountFemale { get; set; }
        public int Index { get; set; }
    }
}
