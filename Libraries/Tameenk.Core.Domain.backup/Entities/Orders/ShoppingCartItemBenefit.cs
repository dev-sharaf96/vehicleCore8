using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities.Orders
{
    public class ShoppingCartItemBenefit : BaseEntity
    {
        public int Id { get; set; }
        public int ShoppingCartItemId { get; set; }
        public long ProductBenefitId { get; set; }

        public ShoppingCartItem ShoppingCartItem { get; set; }
        public Product_Benefit Product_Benefit { get; set; }
    }
}
