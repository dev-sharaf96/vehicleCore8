using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Quotation.Components
{
    public class QuotationsFormTemplateQuoteViewModel
    {
        public string CompanyKey { get; set; }
        public string ImageURL { get; set; }
        public string ProductName { get; set; }
        public string TotalPremium { get; set; }
        public string ClaimLoading { get; set; }
        public string InsurancePercentage { get; set; }
        public string MinimumPremium { get; set; }
        public string ShadowAmount { get; set; }
        public string VAT { get; set; }
        public string LoyalityAmount { get; set; }
        public string Total { get; set; }

        public int? DeductableValue { get; set; }
        public decimal? TotalBenfitPrice { get; set; }
        
        public List<QuotationsFormTemplateQuoteBenfitViewModel> Benefits { get; set; }
    }

    public class QuotationsFormTemplateQuoteBenfitViewModel
    {
        public string BName { get; set; }
        public bool IsChecked { get; set; }
        public decimal? Bprice { get; set; }
    }
}
