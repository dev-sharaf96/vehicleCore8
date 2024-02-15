using System.Collections.Generic;
using Tameenk.Core.Domain.Enums;
using Tamkeen.bll.Model;

namespace Tamkeen.bll.Lookups
{
    public interface IBankLookup
    {
        List<Lookup> GetBanks(LanguageTwoLetterIsoCode language);
    }
}