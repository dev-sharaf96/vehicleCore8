using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TameenkDAL.Store;
using Tamkeen.bll.Model;
using Tameenk.Core.Domain.Enums;

namespace Tamkeen.bll.Lookups
{
    public class SpeedStabilizerLookup : ISpeedStabilizerLookup
    {
        private Dictionary<LanguageTwoLetterIsoCode, List<Lookup>> speedStabilizers = new Dictionary<LanguageTwoLetterIsoCode, List<Lookup>>();
        private LookupsRepository lookupsDAL;
        private static SpeedStabilizerLookup speedStabilizerLookup = null;
        private SpeedStabilizerLookup()
        {
            lookupsDAL = new LookupsRepository();
        }

        public static SpeedStabilizerLookup Instance
        {
            get
            {
                if (speedStabilizerLookup == null)
                {
                    speedStabilizerLookup = new SpeedStabilizerLookup();
                }
                return speedStabilizerLookup;
            }
        }

        public List<Lookup> GetSpeedStabilizers(LanguageTwoLetterIsoCode language)
        {
            if (speedStabilizers.ContainsKey(language))
                return speedStabilizers[language];
            speedStabilizers[language] = lookupsDAL.GetAllSpeedStabilizers().Select(a => new Lookup
            {
                Id = a.Code.ToString(),
                Name = language == LanguageTwoLetterIsoCode.Ar ? a.NameAr : a.NameEn
            }).ToList();

            return speedStabilizers[language];

        }
    }
}
