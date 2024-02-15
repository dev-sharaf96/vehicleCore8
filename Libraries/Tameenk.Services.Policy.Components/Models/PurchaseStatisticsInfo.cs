using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Policy.Components
{
    public class PurchaseStatisticsInfo
    {
        public string ProductDescription { get; set; }
        public string PaymentMethod { get; set; }
        public decimal TotalCommission { get; set; }
        public int TotalCount { get; set; }
    }
}
