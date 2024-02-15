using System.Collections.Generic;

namespace Tameenk.Core.Domain.Dtos
{
    public class ProductOrderModel
    {
        public string ProductNameAr { get; set; }
        public string ProductNameEn { get; set; }
        public List<PriceModel> PriceDetails { get; set; }
        public List<BenefitDetailsModel> Benefits { get; set; }
    }
}
