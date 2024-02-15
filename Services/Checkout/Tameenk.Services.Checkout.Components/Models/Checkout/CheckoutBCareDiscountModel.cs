using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Models.Checkout
{
    public class CheckoutBCareDiscountModel
    {
        public int DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public int DiscountPercentage { get; set; }
    }
}
