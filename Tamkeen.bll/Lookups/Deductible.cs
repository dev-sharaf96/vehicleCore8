using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamkeen.bll.Lookups
{
    public class DeductibleLookup
    {
        public List<int> GetDeductibleValuesList()
        {
            List<int> list = new List<int>()
            {
                250,
                500,
                750,
                1000,
                1500,
                2000,
                2500,
                3000,
                4000,
                5000,
                6000,
                7500,
                10000,
                20000
            };

            return list;
        }
    }
}
