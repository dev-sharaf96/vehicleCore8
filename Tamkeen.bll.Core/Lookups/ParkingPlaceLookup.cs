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
    public class ParkingPlaceLookup: IParkingPlaceLookup
    {
        private Dictionary<LanguageTwoLetterIsoCode, List<Lookup>> parkingPlaces = new Dictionary<LanguageTwoLetterIsoCode, List<Lookup>>();
        private LookupsRepository lookupsDAL;
        private static ParkingPlaceLookup parkingPlaceLookup = null;
        private ParkingPlaceLookup()
        {
            lookupsDAL = new LookupsRepository();
        }

        public static ParkingPlaceLookup Instance
        {
            get
            {
                if (parkingPlaceLookup == null)
                {
                    parkingPlaceLookup = new ParkingPlaceLookup();
                }
                return parkingPlaceLookup;
            }
        }

        public List<Lookup> GetParkingPlaces(LanguageTwoLetterIsoCode language)
        {
            if (parkingPlaces.ContainsKey(language))
                return parkingPlaces[language];
            parkingPlaces[language] = lookupsDAL.GetAllParkingPlaces().Select(a => new Lookup
            {
                Id = a.Code.ToString(),
                Name = language == LanguageTwoLetterIsoCode.Ar ? a.NameAr : a.NameEn
            }).ToList();

            return parkingPlaces[language];
        }
    }
}
