using System;
using System.Collections.Generic;
using System.Text;

namespace Tameenk.Cancellation.Service.Models
{
    public class PolicyCancellationRequest
    {
        public string ReferenceId { get; set; }
        public string RequestNo { get; set; }
        public string PolicyNo { get; set; }
        public string InsuredBankCode { get; set; }
        public string InsuredIBAN { get; set; }
        public string InsuredIBANFileUrl { get; set; }
        public string AlternativePolicyFileUrl { get; set; }

    }
}
