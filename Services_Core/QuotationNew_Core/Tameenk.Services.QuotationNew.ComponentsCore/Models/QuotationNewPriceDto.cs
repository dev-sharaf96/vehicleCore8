using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.QuotationNew.Components
{
    public class QuotationNewPriceDto
    {
        public int PriceTypeCode { get; set; }
        public decimal PriceValue { get; set; }
        public decimal? PercentageValue { get; set; }
    }
}
