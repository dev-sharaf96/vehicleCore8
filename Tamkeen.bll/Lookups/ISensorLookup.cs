using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Enums;
using Tamkeen.bll.Model;

namespace Tamkeen.bll.Lookups
{
    public interface ISensorLookup
    {
        List<Lookup> GetSensors(LanguageTwoLetterIsoCode language);
    }
}
