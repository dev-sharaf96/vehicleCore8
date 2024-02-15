using System;
using System.Collections.Generic;
using Tameenk.Core.Domain.Entities.Policies;

namespace Tameenk.Core.Domain.Entities
{
    public class PayfortPaymentRequest : BaseEntity
    {
        public PayfortPaymentRequest()
        {
            PayfortPaymentResponses = new HashSet<PayfortPaymentResponse>();
        }

        public int ID { get; set; }
        
        public string UserId { get; set; }

        public decimal? Amount { get; set; }
        
        public string ReferenceNumber { get; set; }

        public List<CheckoutDetail> CheckoutDetails { get; set; }
        public List<PolicyUpdateRequest> PolicyUpdateRequests { get; set; }

        public ICollection<PayfortPaymentResponse> PayfortPaymentResponses { get; set; }

        public bool IsCancelled { get; set; }
        public DateTime? CancelationDate { get; set; }
        public string CancelledBy { get; set; }
    }
}
