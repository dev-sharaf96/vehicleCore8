using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities.Orders
{
    public class ShoppingCartItem : BaseEntity
    {
        private ICollection<ShoppingCartItemBenefit> _shoppingCartItemBenefits;
        private ICollection<ShoppingCartItemDriver> _shoppingCartItemDriver;

        public int Id { get; set; }
        public string UserId { get; set; }
        public string ReferenceId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }

        public AspNetUser User { get; set; }
        public Product Product { get; set; }

        public ICollection<ShoppingCartItemBenefit> ShoppingCartItemBenefits
        {
            get { return _shoppingCartItemBenefits ?? (_shoppingCartItemBenefits = new List<ShoppingCartItemBenefit>()); }
            protected set { _shoppingCartItemBenefits = value; }
        }

        public ICollection<ShoppingCartItemDriver> ShoppingCartItemDrivers
        {
            get { return _shoppingCartItemDriver ?? (_shoppingCartItemDriver = new List<ShoppingCartItemDriver>()); }
            protected set { _shoppingCartItemDriver = value; }
        }
    }
}
