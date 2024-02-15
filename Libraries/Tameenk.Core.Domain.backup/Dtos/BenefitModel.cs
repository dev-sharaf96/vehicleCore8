namespace Tameenk.Core.Domain.Dtos
{
    public class BenefitDetailsModel
    {
        public int? BenefitCode { get; set; }
        public string BenefitId { get; set; }
        public string BenefitNameAr { get; set; }
        public string BenefitNameEn { get; set; }
        public string BenefitDescAr { get; set; }
        public string BenefitDescEn { get; set; }
        public decimal? BenefitPrice { get; set; }
        public bool IsSelected { get; set; }
        public int ProductBenefitId { get; set; }
    }
}
