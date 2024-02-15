using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tamkeen.bll.Model;

namespace Tamkeen.bll.Lookups
{
    public interface IYearLookup
    {
        List<Lookup> GetYears();
        List<Lookup> GetArabicYears();
        List<Lookup> GetlicenseYears();
    }
}
