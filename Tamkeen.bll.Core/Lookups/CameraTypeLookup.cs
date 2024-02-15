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
    class CameraTypeLookup : ICameraTypeLookup
    {
        private Dictionary<LanguageTwoLetterIsoCode, List<Lookup>> cameras = new Dictionary<LanguageTwoLetterIsoCode, List<Lookup>>();
        private LookupsRepository lookupsDAL;
        private static CameraTypeLookup cameraLookup = null;
        private CameraTypeLookup()
        {
            lookupsDAL = new LookupsRepository();
        }

        public static CameraTypeLookup Instance
        {
            get
            {
                if (cameraLookup == null)
                {
                    cameraLookup = new CameraTypeLookup();
                }
                return cameraLookup;
            }
        }

        public List<Lookup> GetCameraTypes(LanguageTwoLetterIsoCode language)
        {
            if (cameras.ContainsKey(language))
                return cameras[language];
            cameras[language] = lookupsDAL.GetAllCameras().Select(a => new Lookup
            {
                Id = a.Code.ToString(),
                Name = language == LanguageTwoLetterIsoCode.Ar ? a.NameAr : a.NameEn
            }).ToList();

            return cameras[language];
        }
    }
}
