using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Payment.Esal.Component
{
    public class InvoiceModel
    {
        public string InvoiceNumber { get; set; }
        public string InvoiceId { get; set; }
        public string Status { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
       public DateTime BillDueDate { get; set; }
    }
}
