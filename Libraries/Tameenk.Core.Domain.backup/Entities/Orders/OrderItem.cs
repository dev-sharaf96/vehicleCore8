using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities.Orders
{
    public class OrderItem : BaseEntity
    {
        private ICollection<OrderItemBenefit> _orderItemBenefits;
        private ICollection<OrderItemDriver> _orderItemDrivers;

        public int Id { get; set; }
        public string CheckoutDetailReferenceId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }


        public decimal Price { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }

        public CheckoutDetail CheckoutDetail { get; set; }
        public Product Product { get; set; }

        public ICollection<OrderItemBenefit> OrderItemBenefits
        {
            get { return _orderItemBenefits ?? (_orderItemBenefits = new List<OrderItemBenefit>()); }
            protected set { _orderItemBenefits = value; }
        }

        public ICollection<OrderItemDriver> OrderItemDrivers
        {
            get { return _orderItemDrivers ?? (_orderItemDrivers = new List<OrderItemDriver>()); }
            protected set { _orderItemDrivers = value; }
        }
    }
}
