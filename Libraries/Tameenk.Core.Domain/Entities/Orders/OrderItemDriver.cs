using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tameenk.Core.Domain.Entities.VehicleInsurance;

namespace Tameenk.Core.Domain.Entities.Orders
{
    public class OrderItemDriver : BaseEntity
    {
        public int Id { get; set; }
        public int OrderItemId { get; set; }
        public Guid DriverId { get; set; }
        public decimal Price { get; set; }
        public string DriverExternalId { get; set; }
        public Driver Driver { get; set; }
        public OrderItem OrderItem { get; set; }
    }
}
