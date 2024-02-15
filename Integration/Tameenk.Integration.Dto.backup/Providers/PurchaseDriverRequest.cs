using System;
using System.Collections.Generic;
 
 

namespace Tameenk.Integration.Dto.Providers
{
    public class PurchaseDriverRequest
    {
        public string ReferenceId { set; get; }
        public string PolicyNo { set; get; }      
        public decimal PaymentAmount { set; get; }
        public string PaymentBillNumber { set; get; } 
    }
   
}
