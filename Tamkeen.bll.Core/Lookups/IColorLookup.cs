using System.Collections.Generic;
using Tameenk.Core.Domain.Enums;
using Tamkeen.bll.Model;

namespace Tamkeen.bll.Lookups
{
    public interface IColorLookup
    {
        List<Lookup> GetColors(LanguageTwoLetterIsoCode language);
    }
}
