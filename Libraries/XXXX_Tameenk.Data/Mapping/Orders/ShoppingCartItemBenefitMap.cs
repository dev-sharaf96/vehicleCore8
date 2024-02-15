using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.Orders;

namespace Tameenk.Data.Mapping.Orders
{
    public class ShoppingCartItemBenefitMap : EntityTypeConfiguration<ShoppingCartItemBenefit>
    {
        public ShoppingCartItemBenefitMap()
        {
            ToTable("ShoppingCartItemBenefit");
            HasKey(e => e.Id);

            HasRequired(e => e.ShoppingCartItem).WithMany(e => e.ShoppingCartItemBenefits).HasForeignKey(e => e.ShoppingCartItemId);
            HasRequired(e => e.Product_Benefit).WithMany().HasForeignKey(e => e.ProductBenefitId);
        }
    }
}
