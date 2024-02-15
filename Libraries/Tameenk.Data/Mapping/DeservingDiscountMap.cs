using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities;
using Tameenk.Core.Domain.Entities.PromotionPrograms;

namespace Tameenk.Data.Mapping
{
    public class DeservingDiscountMap : EntityTypeConfiguration<DeservingDiscount>
    {
        public DeservingDiscountMap() {
            ToTable("DeservingDiscount");
            HasKey(c => c.Id);
        }
    }
}