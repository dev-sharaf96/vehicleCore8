using System.Collections.Generic;
using System.Linq;
using Tameenk.Core.Domain.Enums;
using TameenkDAL.Store;
using Tamkeen.bll.Model;

namespace Tamkeen.bll.Lookups
{
    public class ColorLookup : IColorLookup
    {
        private static ColorLookup colorLookup = null;
        private Dictionary<LanguageTwoLetterIsoCode, List<Lookup>> colors = new Dictionary<LanguageTwoLetterIsoCode, List<Lookup>>();
        private LookupsRepository lookupsDAL;
        private ColorLookup()
        {
            lookupsDAL = new LookupsRepository();
        }

        public static ColorLookup Instance
        {
            get
            {
                if(colorLookup == null)
                {
                    colorLookup = new ColorLookup();
                }

                return colorLookup;
            }
        }
        public List<Lookup> GetColors(LanguageTwoLetterIsoCode language)
        {
            if (colors.ContainsKey(language))
                return colors[language];

            colors[language] = lookupsDAL.GetAVehicleColors().Select(a => new Lookup
            {
                Id = a.Code.ToString(),
                Name = language == LanguageTwoLetterIsoCode.Ar ? a.ArabicDescription : a.EnglishDescription
            }).ToList();

            return colors[language];
        }
    }
}
