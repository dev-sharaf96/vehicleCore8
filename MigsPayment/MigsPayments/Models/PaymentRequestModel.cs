using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MigsPayments.Models
{
    public class PaymentRequestModel
    {
        public string Version { get; set; }
        public string Command { get; set; }
        public string AccessCode { get; set; }

        public string SecureHashSecret { get; set; }
        public string MerchTxnRef { get; set; }
        public string MerchantId { get; set; }
        public string OrderInfo { get; set; }
        public string Amount { get; set; }
        public string ReturnUrl { get; set; }
        public string Locale { get; set; }

        public string RedirectUrl { get; set; }
        public string MessageToHash { get; set; }

        public string GeneratedHash { get; set; }
    }
}