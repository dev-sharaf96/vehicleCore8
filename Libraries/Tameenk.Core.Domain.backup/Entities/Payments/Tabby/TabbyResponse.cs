using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities.Payments.Tabby
{
    public class TabbyResponse : BaseEntity
    {
        [Key]
        public Int64 Id { get; set; }
        public virtual Guid TabbyRequestId { get; set; }
        public TabbyRequest TabbyRequest { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;

        public string PaymentId { get; set; }
       // public string Status { get; set; }
       // public string ResponseJson { get; set; }
        public string ReferenceId { get; set; }
        public double TotalAmount { get; set; }
        public double AmountPerInstallment { get; set; }
        public int InstallmentCount { get; set; }
        public double AmountRemaining { get; set; }
        public string Channel { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsWarning { get; set; } = false;
        public string UserId { get; set; } 
    }
}
