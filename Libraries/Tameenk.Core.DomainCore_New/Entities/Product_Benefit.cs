using System;

namespace Tameenk.Core.Domain.Entities
{
    public class Product_Benefit : BaseEntity
    {

        public Product_Benefit()
        {
            IsReadOnly = false;
        }

        public long Id { get; set; }

        public Guid? ProductId { get; set; }

        public short? BenefitId { get; set; }

        public bool? IsSelected { get; set; }

        public decimal? BenefitPrice { get; set; }
        
        public string BenefitExternalId { get; set; }

        public Benefit Benefit { get; set; }

        public Product Product { get; set; }

        public bool IsReadOnly { get; set; }
        public string BenefitNameAr { get; set; }
        public string BenefitNameEn { get; set; }
        public string CoveredCountry { get; set; }
        public decimal? AveragePremium { get; set; }
        public int? Limit { get; set; }
    }
}
