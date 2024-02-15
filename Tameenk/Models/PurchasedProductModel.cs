using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Models
{
    public class PurchasedProductModel
    {
        public string InsuranceCompanyDesc { get; set; }
        public string InsuranceType { get; set; }

        public int InsuraneCompanyId { get; set; }
        public string CompanyMobile { get; set; }
        public string CompanyHomePhone { get; set; }
        public string CompanyFax { get; set; }
        public string CompanyEmail { get; set; }


        public string InsuranceCompanyKey { get; set; }
        public string CheckoutEmail { get; set; }
    }
}