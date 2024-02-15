using System.Collections.Generic;
using System.Linq;
using Tameenk.Core.Domain.Enums;
using TameenkDAL.Store;
using Tamkeen.bll.Model;

namespace Tamkeen.bll.Lookups
{
    public class MonthLookup : IMonthLookup
    {
        private Dictionary<Language, List<Lookup>> arabicMonths = new Dictionary<Language, List<Lookup>>();
        private Dictionary<Language, List<Lookup>> months = new Dictionary<Language, List<Lookup>>();
        private MonthLookup() { lookupsDAL = new LookupsRepository(); }
        private static MonthLookup monthLookup = null;
        public static IMonthLookup Instance
        {
            get
            {
                if(monthLookup == null)
                {
                    monthLookup = new MonthLookup();
                }
                return monthLookup;
            }
        }


        LookupsRepository lookupsDAL;

        
        public List<Lookup> GetArabicMonthes(Language language)
        {
            if (arabicMonths.ContainsKey(language))
            {
                if (arabicMonths[language].Count == 0)
                    arabicMonths.Remove(language);
                else
                    return arabicMonths[language];
            }
            var list  = lookupsDAL.GetAllHejriMonths().Select(a => new Lookup
            {
                Id = a.Id.ToString(),
                Name = language == Language.Ar ? a.Id.ToString() + "-" + a.NameAR : a.Id.ToString() + "-" + a.NameEN
            }).ToList();

            arabicMonths[language] = list;
            return list;

        }

        public List<Lookup> GetMonthes(Language language)
        {
            if (months.ContainsKey(language))
            {
                if (months[language].Count == 0)
                    months.Remove(language);
                else
                    return months[language];
            }
            var list = lookupsDAL.GetAllGregorianMonths().Select(a => new Lookup
            {
                Id = a.Id.ToString(),
                Name = language == Language.Ar ? a.Id.ToString() + "-" + a.NameAR : a.Id.ToString() + "-" + a.NameEN
            }).ToList();

            months[language] = list;
            return list;
        }
    }
}
