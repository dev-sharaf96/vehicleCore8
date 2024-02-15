namespace Tameenk.Core.Domain.Entities
{
    public class YakeenCityCenter : BaseEntity, ILookupTable
    {
        public int Id { get; set; }

        public int CityID { get; set; }

        public string CityName { get; set; }

        public string EnglishName { get; set; }

        public int ZipCode { get; set; }

        public int RegionID { get; set; }

        public string RegionArabicName { get; set; }

        public string RegionEnglishName { get; set; }

        public int ElmCode { get; set; }
    }
}
