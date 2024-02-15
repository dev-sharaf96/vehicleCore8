using System.Collections.Generic;
using Tameenk.Core.Domain.Enums;
using Tamkeen.bll.Model;

namespace Tamkeen.bll.Lookups
{
    public interface IDrivingLicenceYearLookup
    {
        List<Lookup> GetDrivingLicenceYears(LanguageTwoLetterIsoCode language);

    }
}
