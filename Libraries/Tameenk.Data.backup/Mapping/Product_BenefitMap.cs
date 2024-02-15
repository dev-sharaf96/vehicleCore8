using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Services.QuotationApi.Data.Mapping
{
    public class Product_BenefitMap : EntityTypeConfiguration<Product_Benefit>
    {
        public Product_BenefitMap() {

            Property(e => e.BenefitPrice).HasPrecision(19, 4);
            Property(p => p.BenefitExternalId).HasMaxLength(50);
            Property(e => e.BenefitPrice)
                .HasPrecision(19, 4);
        }
    }
}