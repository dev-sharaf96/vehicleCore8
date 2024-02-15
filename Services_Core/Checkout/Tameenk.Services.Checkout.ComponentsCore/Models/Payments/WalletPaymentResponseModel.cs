using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Checkout.Components
{
    public class WalletPaymentResponseModel
    {
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
        public string ReferenceId { get; set; }
        public string NewBalance { get; set; }
    }
}
