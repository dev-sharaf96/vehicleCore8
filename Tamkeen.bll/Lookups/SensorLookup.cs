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
    public class SensorLookup : ISensorLookup
    {
        private Dictionary<LanguageTwoLetterIsoCode, List<Lookup>> sensors = new Dictionary<LanguageTwoLetterIsoCode, List<Lookup>>();
        private LookupsRepository lookupsDAL;
        private static SensorLookup sensorLookup = null;
        private SensorLookup()
        {
            lookupsDAL = new LookupsRepository();
        }

        public static SensorLookup Instance
        {
            get
            {
                if (sensorLookup == null)
                {
                    sensorLookup = new SensorLookup();
                }
                return sensorLookup;
            }
        }

        public List<Lookup> GetSensors(LanguageTwoLetterIsoCode language)
        {
            if (sensors.ContainsKey(language))
                return sensors[language];
            sensors[language] = lookupsDAL.GetllSensors().Select(a => new Lookup
            {
                Id = a.Code.ToString(),
                Name = language == LanguageTwoLetterIsoCode.Ar ? a.NameAr : a.NameEn
            }).ToList();

            return sensors[language];
        }
    }
}
