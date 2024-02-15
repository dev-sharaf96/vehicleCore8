using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.PaymentNotificationsApi.Domain.WafierAndWaffer
{
    public class PaymentRequests
    {
        public int ID { get; set; }
        public int? PaymentMethodsID { get; set; }
        public virtual PaymentMethods PaymentMethod { get; set; }
        public int PaymentsID { get; set; }
        public virtual Payments Payment { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserId { get; set; }
        public virtual ICollection<PaymentRequestDetails> PaymentRequestDetails { get; set; }
    }
}