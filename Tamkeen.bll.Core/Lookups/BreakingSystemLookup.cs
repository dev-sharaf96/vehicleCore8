using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Enums;
using TameenkDAL.Store;
using Tamkeen.bll.Model;

namespace Tamkeen.bll.Lookups
{
    public class BreakingSystemLookup : IBreakingSystemLookup
    {
        private Dictionary<LanguageTwoLetterIsoCode, List<Lookup>> breakingSystems = new Dictionary<LanguageTwoLetterIsoCode, List<Lookup>>();
        private LookupsRepository lookupsDAL;
        private static BreakingSystemLookup breakingLookup = null;
        private BreakingSystemLookup()
        {
            lookupsDAL = new LookupsRepository();
        }

        public static BreakingSystemLookup Instance
        {
            get
            {
                if (breakingLookup == null)
                {
                    breakingLookup = new BreakingSystemLookup();
                }
                return breakingLookup;
            }
        }

        public List<Lookup> GetBreakingSystems(LanguageTwoLetterIsoCode language)
        {
                if (breakingSystems.ContainsKey(language))
                    return breakingSystems[language];
                breakingSystems[language] = lookupsDAL.GetAllBreakingsystems().Select(a => new Lookup
                {
                    Id = a.Code.ToString(),
                    Name = language == LanguageTwoLetterIsoCode.Ar ? a.NameAr : a.NameEn
                }).ToList();

                return breakingSystems[language];
        }
    }
}
