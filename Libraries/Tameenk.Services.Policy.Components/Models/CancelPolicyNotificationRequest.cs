using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.Policy.Components
{
    public class CancelPolicyNotificationRequest
    {
        public string ReferenceId { get; set; }
        public string PolicyNo { get; set; }
        public string CancellationDate { get; set; }
        public decimal GrossRefundAmount { get; set; }
        public decimal NetRefundCommission { get; set; }
        public decimal RefundCommission { get; set; }


    }
}