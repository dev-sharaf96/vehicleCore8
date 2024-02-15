using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Payment.Esal.Component
{
    public class EsalUploadInvoiceNotification
    {
        public string SupplierId { get; set; }
        public List<EsalInvoiceNotification> Invoices { get; set; }
    }
}