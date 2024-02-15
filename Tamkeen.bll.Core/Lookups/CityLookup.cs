using System.Collections.Generic;
using System.Linq;
using Tameenk.Core.Domain.Enums;
using TameenkDAL.Store;
using Tamkeen.bll.Model;

namespace Tamkeen.bll.Lookups
{
    public class CityLookup : ICityLookup
    {
        private LookupsRepository lookupsDAL;
        private Dictionary<LanguageTwoLetterIsoCode, List<Lookup>> cities = new Dictionary<LanguageTwoLetterIsoCode, List<Lookup>>();
        private CityLookup()
        {
            lookupsDAL = new LookupsRepository();
        }

        private static CityLookup cityLookup = null;

        public static CityLookup Instance
        {
            get
            {
                if (cityLookup == null)
                    cityLookup = new CityLookup();
                return cityLookup;
            }
        }

        public List<Lookup> GetCities(LanguageTwoLetterIsoCode language)
        {
            if (cities.ContainsKey(language))
                return cities[language];
            cities[language] = lookupsDAL.GetAllCities().Select(a => new Lookup
            {
                Id = a.Code.ToString(),
                Name = language == LanguageTwoLetterIsoCode.Ar ? a.ArabicDescription : a.EnglishDescription
            }).ToList();

            return cities[language];
            //lookup.Add(new Lookup { Id = "1", Name = "الرياض" });
            //lookup.Add(new Lookup { Id = "2", Name = "جده" });
            //lookup.Add(new Lookup { Id = "3", Name = "القسيم" });
        }
    }
}
