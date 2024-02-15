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
    public class DriverMedicalConditionLookup : IDriverMedicalConditionLookup
    {
        private Dictionary<LanguageTwoLetterIsoCode, List<Lookup>> driverMedicalConditions = new Dictionary<LanguageTwoLetterIsoCode, List<Lookup>>();
        private LookupsRepository lookupsDAL;
        private static DriverMedicalConditionLookup driverMedicalConditionsLookup = null;
        private DriverMedicalConditionLookup()
        {
            lookupsDAL = new LookupsRepository();
        }

        public static DriverMedicalConditionLookup Instance
        {
            get
            {
                if (driverMedicalConditionsLookup == null)
                {
                    driverMedicalConditionsLookup = new DriverMedicalConditionLookup();
                }
                return driverMedicalConditionsLookup;
            }
        }

        public List<Lookup> GetDriverMedicalContitions(LanguageTwoLetterIsoCode language)
        {
            if (driverMedicalConditions.ContainsKey(language))
                return driverMedicalConditions[language];
            driverMedicalConditions[language] = lookupsDAL.GetAllDriverMedicalConditions().Select(a => new Lookup
            {
                Id = a.Code.ToString(),
                Name = language == LanguageTwoLetterIsoCode.Ar ? a.NameAr : a.NameEn
            }).ToList();

            return driverMedicalConditions[language];
        }
    }
}
