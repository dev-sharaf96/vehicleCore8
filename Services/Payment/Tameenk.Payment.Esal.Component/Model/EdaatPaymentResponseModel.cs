using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Payment.Esal.Component.Model
{
    public class EdaatPaymentResponseModel
    {
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime BillDueDate { get; set; }
        public string InvoiceNo { set; get; }
        public string ReferenceId { get; set; }
    }
}
