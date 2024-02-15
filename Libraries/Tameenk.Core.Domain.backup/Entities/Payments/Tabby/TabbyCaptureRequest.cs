using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities.Payments.Tabby
{
    public class TabbyCaptureRequest : BaseEntity
    {
        public Int64 Id { get; set; }
        public virtual Guid TabbyRequestId { get; set; }
        public TabbyRequest TabbyRequest { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public string UserId { get; set; }
        public double Amount { get; set; }
        public double TaxAmount { get; set; }
        public double DiscountAmount { get; set; }
        public double ShippingAmount { get; set; }
        public string CreatedAt { get; set; }
        public string Items { get; set; }
        public string Channel { get; set; }
        public string RefrenceId { get; set; }
        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
    }
}
