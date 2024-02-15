using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities.Payments.Tabby
{
    public class TabbyCaptureResponse:BaseEntity
    {
        public Int64 Id { get; set; }
        public virtual Int64 TabbyCaptureRequestId { get; set; }
        public TabbyCaptureRequest TabbyCaptureRequest { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public string CaptureId { get; set; }
        public string CreatedAt { get; set; }
        public string ExpiresAt { get; set; }
        public bool Test { get; set; }
        public bool IsExpired { get; set; }
        public string Status { get; set; }
        public bool Cancelable { get; set; }
        public string Currency { get; set; }
        public double Amount { get; set; }
        public string OrderRefrenceId { get; set; }
        public string RefrenceId { get; set; }
        public string UserId { get; set; }
        public string Channel { get; set; }
        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
    }
}
