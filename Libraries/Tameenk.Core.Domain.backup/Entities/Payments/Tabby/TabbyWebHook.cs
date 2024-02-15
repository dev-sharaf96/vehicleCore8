using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities.Payments.Tabby
{
    public class TabbyWebHook : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }
        public virtual Guid TabbyRequestId { get; set; }
        public TabbyRequest TabbyRequest { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public string InvoiceNo { get; set; }
        public string PaymentId { get; set; }
        public string CreatedAt { get; set; }
        public string ExpiresAt { get; set; }
        public string UserId { get; set; }
        public string MerchantId { get; set; }
        public bool IsTest { get; set; }
        public string ClosedAt { get; set; }
        public string status { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public bool Cancelable { get; set; }
        public string ProductOptionId { get; set; }
        public double Amount { get; set; }
        public string Channel { get; set; }
        public string RefrenceId { get; set; }
        public int? ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
    }
}
