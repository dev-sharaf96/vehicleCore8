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
    public class CountryLookup : ICountryLookup
    {
       

        private Dictionary<LanguageTwoLetterIsoCode, List<Lookup>> countries = new Dictionary<LanguageTwoLetterIsoCode, List<Lookup>>();
        private LookupsRepository lookupsDAL;
        private static CountryLookup countryLookup = null;
        private CountryLookup()
        {
            lookupsDAL = new LookupsRepository();
        }

        public static CountryLookup Instance
        {
            get
            {
                if (countryLookup == null)
                {
                    countryLookup = new CountryLookup();
                }
                return countryLookup;
            }
        }
        public List<Lookup> GetCountries(LanguageTwoLetterIsoCode language)
        {
            if (countries.ContainsKey(language))
                return countries[language];
            countries[language] = lookupsDAL.GetAllCountries().Select(a => new Lookup
            {
                Id = a.Code.ToString(),
                Name = language == LanguageTwoLetterIsoCode.Ar ? a.ArabicDescription : a.EnglishDescription
            }).ToList();

            return countries[language];
        }

    }
}
