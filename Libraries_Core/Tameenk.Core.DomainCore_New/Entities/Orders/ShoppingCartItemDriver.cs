using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities.Orders
{
    public class ShoppingCartItemDriver : BaseEntity
    {
        public int Id { get; set; }
        public int ShoppingCartItemId { get; set; }
        public int ProductDriverId { get; set; }

        public ShoppingCartItem ShoppingCartItem { get; set; }
        public Product_Driver Product_Driver { get; set; }
    }
}
