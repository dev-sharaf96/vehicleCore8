namespace Tameenk.Core.Domain.Entities
{
    public class CityCenter : BaseEntity, ILookupTable
    {
        public int Id { get; set; }

        public string CityId { get; set; }

        public string ArabicName { get; set; }

        public string EnglishName { get; set; }

        public string RegionId { get; set; }

        public string RegionArabicName { get; set; }

        public string RegionEnglishName { get; set; }

        public string ELM_Code { get; set; }
        public bool IsActive { get; set; }

    }
}
