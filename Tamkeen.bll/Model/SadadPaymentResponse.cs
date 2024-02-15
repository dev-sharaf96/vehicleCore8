using System;
using Tameenk.Core.Domain.Enums;

namespace Tamkeen.bll.Model
{
    public class SadadPaymentResponse
    {
        public EStatus Status { get; set; }

        public string ErrorMessage { get; set; }

        public string ReferenceNumber { get; set; }

        public DateTime BillDueDate { get; set; }
    }
}
