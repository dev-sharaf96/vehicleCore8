using System.Collections.Generic;
using Tameenk.Core.Domain.Enums;
using Tamkeen.bll.Model;

namespace Tamkeen.bll.Lookups
{
    public interface IMonthLookup
    {
        List<Lookup> GetMonthes(LanguageTwoLetterIsoCode language);
        List<Lookup> GetArabicMonthes(LanguageTwoLetterIsoCode language);
    }
}
