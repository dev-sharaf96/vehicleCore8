using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto.Providers
{
    public class PriceDto
    {
        public int PriceTypeCode { get; set; }
        public decimal PriceValue { get; set; }
        public decimal? PercentageValue { get; set; }
    }
}
