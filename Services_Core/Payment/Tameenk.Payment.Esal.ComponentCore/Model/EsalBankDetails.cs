using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Payment.Esal.Component
{
    public class EsalBankDetails
    {
        public string BankId { get; set; }
        public string BranchCode { get; set; }
        public string DistrictCode { get; set; }
        public string AccessChannel { get; set; }
    }
}