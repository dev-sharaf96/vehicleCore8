using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Models
{
    public class SadadPaymentViewModel
    {
        public string ReferenceId { get; set; }

        public string CustomerName { get; set; }

        public decimal PaymentAmount { get; set; }
    }
}