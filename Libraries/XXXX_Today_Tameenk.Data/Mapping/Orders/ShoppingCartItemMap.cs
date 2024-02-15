using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.Orders;

namespace Tameenk.Data.Mapping.Orders
{
    public class ShoppingCartItemMap : EntityTypeConfiguration<ShoppingCartItem>
    {
        public ShoppingCartItemMap()
        {
            ToTable("ShoppingCartItem");
            HasKey(e => e.Id);

            HasRequired(e => e.Product).WithMany().HasForeignKey(e => e.ProductId);
            HasRequired(e => e.User).WithMany().HasForeignKey(e => e.UserId);
        }
    }
}
