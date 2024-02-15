using System.Data.Entity.ModelConfiguration;
using Tameenk.Core.Domain.Entities.Orders;

namespace Tameenk.Data.Mapping.Orders
{
    public class OrderItemMap : EntityTypeConfiguration<OrderItem>
    {
        public OrderItemMap()
        {
            ToTable("OrderItem");
            HasKey(e => e.Id);


            Property(p => p.Price).HasPrecision(19, 4);
            HasRequired(e => e.Product).WithMany().HasForeignKey(e => e.ProductId);
            HasRequired(e => e.CheckoutDetail).WithMany(e => e.OrderItems).HasForeignKey(e => e.CheckoutDetailReferenceId);
        }
    }
}
