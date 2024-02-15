using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Quotation.Components
{
    public class QuotationsFormPriceDetailsDto
    {
        public Guid? ProductId { get; set; }
        public Byte? PriceTypeCode { get; set; }
        public decimal? PriceValue { get; set; }
        public decimal? PercentageValue { get; set; }
        public string PriceTypeArabicDescription { get; set; }
        public string PriceTypeEnglishDescription { get; set; }
    }
}
