using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;

namespace Tameenk.Services.QuotationApi.Data.Mapping
{
    public class Quotation_Product_BenefitMap : EntityTypeConfiguration<Quotation_Product_Benefit>
    {
        public Quotation_Product_BenefitMap() {

            Property(e => e.BenefitPrice).HasPrecision(19, 4);
            Property(p => p.BenefitExternalId).HasMaxLength(50);
        }
    }
}