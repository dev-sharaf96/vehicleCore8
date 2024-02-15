using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Core.Payments
{
    public class SadadPaymentResponse
    {
        public SadadPaymentStatus Status { get; set; }

        public string ErrorMessage { get; set; }

        public string ReferenceNumber { get; set; }

        public DateTime BillDueDate { get; set; }
    }

    public enum SadadPaymentStatus {
        Succeeded,
        Failed
    }
}
