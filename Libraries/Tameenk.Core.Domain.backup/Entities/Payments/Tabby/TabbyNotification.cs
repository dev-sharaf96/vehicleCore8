using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities.Payments.Tabby
{
    public class TabbyNotification : BaseEntity
    {
        [Key]
        public Int64 Id { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public virtual Guid TabbyRequestId { get; set; }
        public TabbyRequest TabbyRequest { get; set; }
        public string InvoiceNo { get; set; }
        public string ReferenceID { get; set; }
        public string Channel { get; set; }
        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string PaymentId { get; set; }
        public string CreatedAT { get; set; }
        public string ExpiresAt { get; set; }
        public string Status { get; set; }
        public bool? IsTest { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public string Meta { get; set; }
        public string Description { get; set; }
        public bool? Cancelable { get; set; }
        public string UserId { get; set; }

    }
}
