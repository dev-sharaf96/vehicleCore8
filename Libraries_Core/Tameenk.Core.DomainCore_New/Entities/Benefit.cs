using System.Collections.Generic;

namespace Tameenk.Core.Domain.Entities
{
    public class Benefit : BaseEntity
    {
        public Benefit()
        {
            InsuaranceCompanyBenefits = new HashSet<InsuaranceCompanyBenefit>();
            Invoice_Benefits = new HashSet<Invoice_Benefit>();
            Product_Benefits = new HashSet<Product_Benefit>();
            Quotation_Product_Benefits = new HashSet<Quotation_Product_Benefit>();
        }
        
        public short Code { get; set; }
        
        public string EnglishDescription { get; set; }
        
        public string ArabicDescription { get; set; }

        public ICollection<InsuaranceCompanyBenefit> InsuaranceCompanyBenefits { get; set; }

        public ICollection<Invoice_Benefit> Invoice_Benefits { get; set; }

        public ICollection<Product_Benefit> Product_Benefits { get; set; }
        public ICollection<Quotation_Product_Benefit> Quotation_Product_Benefits { get; set; }
    }
}
