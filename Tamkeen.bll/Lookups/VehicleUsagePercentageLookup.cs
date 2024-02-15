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
    public class VehicleUsagePercentageLookup : IVehicleUsagePercentageLookup
    {

        private Dictionary<LanguageTwoLetterIsoCode, List<Lookup>> vehicleUsagePercentages = new Dictionary<LanguageTwoLetterIsoCode, List<Lookup>>();
        private LookupsRepository lookupsDAL;
        private static VehicleUsagePercentageLookup vehicleUsagePercentagesLookup = null;
        private VehicleUsagePercentageLookup()
        {
            lookupsDAL = new LookupsRepository();
        }

        public static VehicleUsagePercentageLookup Instance
        {
            get
            {
                if (vehicleUsagePercentagesLookup == null)
                {
                    vehicleUsagePercentagesLookup = new VehicleUsagePercentageLookup();
                }
                return vehicleUsagePercentagesLookup;
            }
        }

        public List<Lookup> GetVehicleUsagePercentages(LanguageTwoLetterIsoCode language)
        {
            if (vehicleUsagePercentages.ContainsKey(language))
                return vehicleUsagePercentages[language];
            vehicleUsagePercentages[language] = lookupsDAL.GetAllVehicleUsagePercentages().Select(a => new Lookup
            {
                Id = a.Code.ToString(),
                Name = language == LanguageTwoLetterIsoCode.Ar ? a.NameAr : a.NameEn
            }).ToList();

            return vehicleUsagePercentages[language];
        }
    }
}
