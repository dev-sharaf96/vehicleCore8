using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.YakeenIntegration.Business
{
    public class YakeenAddressResult
    {
        public int AdditionalNumber { get; set; }

        public int BuildingNumber { get; set; }

        public string City { get; set; }

        public string District { get; set; }

        public string LocationCoordinates { get; set; }

        public int PostCode { get; set; }

        public string StreetName { get; set; }

        public int UnitNumber { get; set; }
        public bool IsPrimaryAddress { get; set; }
        
    }
}
