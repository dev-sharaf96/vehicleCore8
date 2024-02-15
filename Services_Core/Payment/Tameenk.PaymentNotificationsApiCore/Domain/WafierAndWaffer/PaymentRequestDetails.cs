using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.PaymentNotificationsApi.Domain.WafierAndWaffer
{
    public class PaymentRequestDetails
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public int PaymentRequestsID { get; set; }
        public virtual PaymentRequests PaymentRequest { get; set; }
    }
}