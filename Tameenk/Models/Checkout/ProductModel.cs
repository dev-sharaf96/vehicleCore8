using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tameenk.Core.Domain.Enums;

namespace Tameenk.Models.Checkout
{
    public class ProductModel
    {
        public string ProductNameAr { get; set; }
        public string ProductNameEn { get; set; }
        public List<PriceModel> PriceDetails { get; set; }
        public List<BenefitModel> Benefits { get; set; }
    }
}