using System.Collections.Generic;
using Tameenk.Core.Domain.Entities.Quotations;

namespace Tameenk.Core.Domain.Entities
{
    public class ProductType : BaseEntity
    {
        public ProductType()
        {
            CheckoutDetails = new HashSet<CheckoutDetail>();
            InsuranceCompanyProductTypeConfigs = new HashSet<InsuranceCompanyProductTypeConfig>();
            Invoices = new HashSet<Invoice>();
            QuotationResponses = new HashSet<QuotationResponse>();
        }
        
        public short Code { get; set; }
        
        public string EnglishDescription { get; set; }
        
        public string ArabicDescription { get; set; }

        public ICollection<CheckoutDetail> CheckoutDetails { get; set; }

        public ICollection<InsuranceCompanyProductTypeConfig> InsuranceCompanyProductTypeConfigs { get; set; }

        public ICollection<Invoice> Invoices { get; set; }

        public ICollection<QuotationResponse> QuotationResponses { get; set; }
    }
}
