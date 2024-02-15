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
    class DrivingLicenceYearLookup : IDrivingLicenceYearLookup
    {
        public List<Lookup> GetDrivingLicenceYears(LanguageTwoLetterIsoCode language)
        {
            if (drivingLicenceYears.ContainsKey(language))
                return drivingLicenceYears[language];
            drivingLicenceYears[language] = lookupsDAL.GetAllDrivingLicenceYears().Select(a => new Lookup
            {
                Id = a.Code.ToString(),
                Name = language == LanguageTwoLetterIsoCode.Ar ? a.NameAr : a.NameEn
            }).ToList();

            return drivingLicenceYears[language];
        }

        private Dictionary<LanguageTwoLetterIsoCode, List<Lookup>> drivingLicenceYears = new Dictionary<LanguageTwoLetterIsoCode, List<Lookup>>();
        private LookupsRepository lookupsDAL;
        private static DrivingLicenceYearLookup drivingLicenceYearsLookup = null;
        private DrivingLicenceYearLookup()
        {
            lookupsDAL = new LookupsRepository();
        }

        public static DrivingLicenceYearLookup Instance
        {
            get
            {
                if (drivingLicenceYearsLookup == null)
                {
                    drivingLicenceYearsLookup = new DrivingLicenceYearLookup();
                }
                return drivingLicenceYearsLookup;
            }
        }
      
    }
}
