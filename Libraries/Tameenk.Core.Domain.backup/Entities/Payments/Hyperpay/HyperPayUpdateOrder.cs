using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Domain.Entities
{
    public class HyperPayUpdateOrder : BaseEntity
    {
        public int Id { get; set; }
        public bool? IsBcareUpdateOrder { get; set; }
        public string ServiceRequest { get; set; }
        public string RequestAmount { get; set; }
        public string RequestConfigId { get; set; }
        public string RequestUniqueId { get; set; }
        public string RequestBeneficiaryAccountId { get; set; }
        public string RequestPaymentBrand { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ReferenceId { get; set; }
        public string Channel { get; set; }
        public bool? ResponseStatus { get; set; }
        public string ResponseMessage { get; set; }
        public string ResponseAmount { get; set; }
        public string PayoutStatus { get; set; }
        public string PayoutBeneficiaryName { get; set; }
        public string PaymentType { get; set; }
        public string ServiceResponse { get; set; }
    }
}
