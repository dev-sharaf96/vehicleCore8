using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities.Payments.Tabby
{
    public  class TabbyRequestDetails : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public virtual Guid TabbyRequestId { get; set; }
        public TabbyRequest TabbyRequest { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;

        public string Buyer { get; set; }
        public string ShippingAddress { get; set; }
        public string Order { get; set; }
        public string buyerHistory { get; set; }
        public string OrderHistory { get; set; }
        public string Meta { get; set; }
        public string Attachment { get; set; }
        public string Lang { get; set; }
        public string MerchantCode { get; set; }
        public string MerchantURL { get; set; }

    }
}
