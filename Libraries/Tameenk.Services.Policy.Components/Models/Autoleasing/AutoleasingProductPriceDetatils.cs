using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Policy.Components
{
    public class AutoleasingProductPriceDetatils
    {
        public Guid ProductID { get; set; }

        public byte PriceTypeCode { get; set; }

        public decimal PriceValue { get; set; }

        public decimal? PercentageValue { get; set; }
    }
}
