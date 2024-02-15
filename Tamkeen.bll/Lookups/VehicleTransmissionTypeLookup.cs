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
    class VehicleTransmissionTypeLookup : IVehicleTransmissionTypeLookup
    {
        private Dictionary<LanguageTwoLetterIsoCode, List<Lookup>> transmissionTypes = new Dictionary<LanguageTwoLetterIsoCode, List<Lookup>>();
        private LookupsRepository lookupsDAL;
        private static VehicleTransmissionTypeLookup vehicleTransmissionTypeLookup = null;
        private VehicleTransmissionTypeLookup()
        {
            lookupsDAL = new LookupsRepository();
        }

        public static VehicleTransmissionTypeLookup Instance
        {
            get
            {
                if (vehicleTransmissionTypeLookup == null)
                {
                    vehicleTransmissionTypeLookup = new VehicleTransmissionTypeLookup();
                }
                return vehicleTransmissionTypeLookup;
            }
        }

        public List<Lookup> GetTransmissionTypes(LanguageTwoLetterIsoCode language)
        {
            if (transmissionTypes.ContainsKey(language))
                return transmissionTypes[language];
            transmissionTypes[language] = lookupsDAL.GetAllTransmissionTypes().Select(a => new Lookup
            {
                Id = a.Code.ToString(),
                Name = language == LanguageTwoLetterIsoCode.Ar ? a.NameAr : a.NameEn
            }).ToList();

            return transmissionTypes[language];
        }
    }
}
