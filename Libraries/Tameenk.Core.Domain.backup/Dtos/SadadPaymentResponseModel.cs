using System;

namespace Tameenk.Core.Domain.Dtos
{
    public class SadadPaymentResponseModel
    {
        public string Status { get; set; }

        public string ErrorMessage { get; set; }

        public string ReferenceNumber { get; set; }

        public DateTime BillDueDate { get; set; }
    }
}
