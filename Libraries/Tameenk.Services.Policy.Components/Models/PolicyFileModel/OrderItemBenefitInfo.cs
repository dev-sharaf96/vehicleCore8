using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Policy.Components
{
    public class OrderItemBenefitInfo
    {
        public short BenefitId { get; set; }
        public decimal Price { get; set; }
        public string BenefitExternalId { get; set; }
        public short Code { get; set; }
        public string EnglishDescription { get; set; }
        public string ArabicDescription { get; set; }
    }
}
