using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Policies
{
    public class CountOfPolicesPerRegion
    {
        public string Region { get; set; }
        public int PolicesIndividual { get; set; }
        public int PolicesCorporate { get; set; }
    }
}
