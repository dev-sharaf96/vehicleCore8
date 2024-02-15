using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Checkout.Components
{
    public class EdaatPaymentResponseModel
    {
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime BillDueDate { get; set; }
        public string InvoiceNo { set; get; }
        public string ReferenceId { get; set; }
        public bool IsCheckoutEmailVerified { get; internal set; }
        public string CheckoutEmail { get; internal set; }
        public string CheckoutReferenceId { get; internal set; }
        public string CompanyName { get; set; }
        public string PremiumAmount { get; set; }
    }
}
