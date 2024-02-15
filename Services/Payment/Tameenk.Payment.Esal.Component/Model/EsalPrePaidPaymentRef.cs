using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Payment.Esal.Component
{
    public class EsalPrePaidPaymentRef
    {
        public string BillingAcct { get; set; }
        public string BeneficiaryPhoneNum { get; set; }
        public string ServiceCode { get; set; }
        public EsalPayor BeneficiaryId { get; set; }
        public string ChkDigit { get; set; }
    }
}