using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Payment.Esal.Component
{
    public class EsalPayment
    {
        public EsalBankDetails BankDetails { get; set; }
        public string BillerId { get; set; }
        public string CurAmt { get; set; }
        public string PmtRefInfo { get; set; }
        public EsalPayor Payor { get; set; }
        public string PrcDt { get; set; }
        public string DueDt { get; set; }
        public string EfftDt { get; set; }
        public string SadadPmtId { get; set; }
        public string BankPmtId { get; set; }
        public string BankReversalId { get; set; }
        public string PmtMethod { get; set; }
        public EsalPayor ProxyPayor { get; set; }
        public string Status { get; set; }
        public EsalPrePaidPaymentRef PrePaidPaymentRef { get; set; }
        public EsalPostPaidPaymentRef PostPaidPaymentRef { get; set; }
    }
}