using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Models.Checkout
{
    public class BenefitModel
    {
        public int? BenefitCode { get; set; }
        public string BenefitId { get; set; }
        public string BenefitNameAr { get; set; }
        public string BenefitNameEn { get; set; }
        public string BenefitDescAr { get; set; }
        public string BenefitDescEn { get; set; }
        public decimal? BenefitPrice { get; set; }
        public bool IsSelected { get; set; }
        public bool IsReadOnly { get; set; }
        public int ProductBenefitId { get; set; }
    }
}