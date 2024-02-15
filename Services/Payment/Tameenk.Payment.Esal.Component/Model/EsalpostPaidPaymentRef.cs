using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Payment.Esal.Component
{
    public class EsalPostPaidPaymentRef
    {
        public string BillingAcct { get; set; }
        public string BillNumber { get; set; }
        public EsalbillNumberWithAccount BillNumberWithAccount { get; set; }
        public string BillCycle { get; set; }
        public string ServiceCode { get; set; }
        public string ChkDigit { get; set; }
    }
}