using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Models.Payments.Sadad
{
    public class SadadPaymentResponseModel
    {
        public string Status { get; set; }

        public string ErrorMessage { get; set; }

        public string ReferenceNumber { get; set; }

        public DateTime BillDueDate { get; set; }
    }
}