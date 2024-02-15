using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities.Payments.Tabby
{
    public class TabbyNotificationDetails : BaseEntity
    {
        public Int64 Id { get; set; }
        public virtual Int64 TabbyNotificationId { get; set; }
        public TabbyNotification TabbyResponse { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string Product { get; set; }
        public string Buyer { get; set; }
        public string ShippingAddress { get; set; }
        public string Order { get; set; }
        public string BuyerHistory { get; set; }
        public string OrderHistory { get; set; }
        public string Refunds { get; set; }
        public string Captures { get; set; }
    }
}
