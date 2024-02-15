using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities.Orders;

namespace Tameenk.Data.Mapping.Orders
{
    public class OrderItemBenefitMap : EntityTypeConfiguration<OrderItemBenefit>
    {
        public OrderItemBenefitMap()
        {
            ToTable("OrderItemBenefit");
            HasKey(e => e.Id);

            HasRequired(e => e.OrderItem).WithMany(e => e.OrderItemBenefits).HasForeignKey(e => e.OrderItemId);
            HasRequired(e => e.Benefit).WithMany().HasForeignKey(e => e.BenefitId);
        }
    }
}
