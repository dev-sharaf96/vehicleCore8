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
    class DistanceRangeLookup : IDistanceRangeLookup
    {
        private Dictionary<LanguageTwoLetterIsoCode, List<Lookup>> distanceRanges = new Dictionary<LanguageTwoLetterIsoCode, List<Lookup>>();
        private LookupsRepository lookupsDAL;
        private static DistanceRangeLookup rangesLookup = null;
        private DistanceRangeLookup()
        {
            lookupsDAL = new LookupsRepository();
        }

        public static DistanceRangeLookup Instance
        {
            get
            {
                if (rangesLookup == null)
                {
                    rangesLookup = new DistanceRangeLookup();
                }
                return rangesLookup;
            }
        }

        public List<Lookup> GetDistanceRanges(LanguageTwoLetterIsoCode language)
        {
            if (distanceRanges.ContainsKey(language))
                return distanceRanges[language];
            distanceRanges[language] = lookupsDAL.GetAllDistanceRanges().Select(a => new Lookup
            {
                Id = a.Code.ToString(),
                Name = language == LanguageTwoLetterIsoCode.Ar ? a.NameAr : a.NameEn
            }).ToList();

            return distanceRanges[language];
        }
    }
}
