using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
    public class HyperPayCreateOrder : BaseEntity
    {
        public int Id { get; set; }
        public string ServiceRequest { get; set; }
        public string RequestConfigId { get; set; }
        public string MerchantTransactionId { get; set; }
        public string UniqueId { get; set; }
        public string Period { get; set; }
        public string TransferOption { get; set; }
        public string RequestPaymentBrand { get; set; }
        public string ReferenceId { get; set; }
        public string Channel { get; set; }
        public string ServiceResponse { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool ResponseStatus { get; set; }
        public string ResponseMessage { get; set; }
    }
}
