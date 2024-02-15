using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Implementation.Policies
{
   public class CountPerPaymentMethod
    {
        public int IndexCount { get; set; }
        public string PaymentMethod { get; set; }
        public int CountOfPolices { get; set; }
    }
}
