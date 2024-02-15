using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Inquiry.Components
{
    public class DriverCityInfoModel
    {
        public long ElmCode { get; set; }
        public int AddressId { get; set; }
        public string PostCode { get; set; }
        public string CityNameEn { get; set; }
        public string CityNameAr { get; set; }
    }
}
