using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Payment.Esal.Component
{
    public class EsalInvoiceNotification
    {
        public string InvoiceNumber { get; set; }
        public string Status { get; set; }
        public string SadadBillsId { get; set; }
        public List<EsalError> Errors { get; set; }
    }
}