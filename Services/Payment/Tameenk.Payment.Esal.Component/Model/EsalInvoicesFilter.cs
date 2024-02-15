using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Payment.Esal.Component
{
    public class EsalInvoicesFilter
    {
        public string InvoiceNumber { get; set; }
        public string SadadBillsNumber { get; set; }
        public string ReferenceId { get; set; }

        public bool? IsPaid { get; set; }

    }
}
