using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities.Payments.Tabby
{
    public class TabbyCaptureResponseDetails : BaseEntity
    {
        public Int64 Id { get; set; }
        public virtual Int64 TabbyCaptureResponseId { get; set; }
        public TabbyCaptureResponse TabbyCaptureResponse { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public string Buyer { get; set; }
        public string Product { get; set; }
        public string ShippingAddress { get; set; }
        public string Order { get; set; }
        public string Captures { get; set; }
        public string Refund { get; set; }
        public string BuyerHistory { get; set; }
        public string OrderHistory { get; set; }
        public string Meta { get; set; }
    }
}
