using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Loggin.DAL.Dtos
{
    public class YakeenCityCenterModel
    {
        public int Id { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; }
        public string EnglishName { get; set; }
        public int ZipCode { get; set; }
        public int RegionId { get; set; }
        public string RegionArabicName { get; set; }
        public string RegionEnglishName { get; set; }
        public int ElmCode { get; set; }
    }
}
