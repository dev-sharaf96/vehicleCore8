using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.PaymentNotificationsApi.Models
{
    public class EdaatPayment
    {
        public string BillNo { get; set; }
        public string InvoiceNo { get; set; }
        public string InternalCode { get; set; }
        public string PaymentDate { get; set; }
        public decimal PaymentAmount { get; set; }
    }
}