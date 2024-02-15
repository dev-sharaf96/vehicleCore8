using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Payment.Esal.Component
{
    public class EsalPaymentResponseModel
    {
        public List<InvoiceModel> invoices { get; set; }
        public string AccessToken { get; set; }
        public string ReferenceId { get; set; }
    }
}
