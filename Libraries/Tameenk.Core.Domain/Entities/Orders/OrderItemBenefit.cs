using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities.Orders
{
    public class OrderItemBenefit : BaseEntity
    {
        public int Id { get; set; }
        public int OrderItemId { get; set; }
        public short BenefitId { get; set; }
        public decimal Price { get; set; }
        public string BenefitExternalId { get; set; }
        public Benefit Benefit { get; set; }
        public OrderItem OrderItem { get; set; }
    }
}
