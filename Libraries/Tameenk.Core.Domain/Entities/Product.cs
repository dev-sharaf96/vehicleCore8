using System;
using System.Collections.Generic;
using Tameenk.Core.Domain.Entities.Quotations;

namespace Tameenk.Core.Domain.Entities
{
    public class Product : BaseEntity
    {
        public Product()
        {
            PriceDetails = new HashSet<PriceDetail>();
            Product_Benefits = new HashSet<Product_Benefit>();
            Quotation_Product_Benefits = new HashSet<Quotation_Product_Benefit>();
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        
        public string ExternalProductId { get; set; }
        
        public string QuotaionNo { get; set; }

        public DateTime? QuotationDate { get; set; }

        public DateTime? QuotationExpiryDate { get; set; }

        public int? ProviderId { get; set; }

        public string ProductNameAr { get; set; }

        public string ProductNameEn { get; set; }

        public string ProductDescAr { get; set; }

        public string ProductDescEn { get; set; }

        public decimal ProductPrice { get; set; }

        public int? DeductableValue { get; set; }

        public int? VehicleLimitValue { get; set; }

        public long? QuotationResponseId { get; set; }
        
        public string ProductImage { get; set; }
        public string ReferenceId { get; set; }
        public int? InsuranceTypeCode { get; set; }
        public bool IsCheckedOut { get; set; }
        public DateTime CreateDateTime { get; set; }
        public decimal? InsurancePercentage { get; set; }
        public decimal? ShadowAmount { get; set; }
        public virtual InsuranceCompany InsuranceCompany { get; set; }

        public ICollection<PriceDetail> PriceDetails { get; set; }

        public ICollection<Product_Benefit> Product_Benefits { get; set; }
        public ICollection<Quotation_Product_Benefit> Quotation_Product_Benefits { get; set; }

        public virtual QuotationResponse QuotationResponse { get; set; }

        public bool IsPromoted { get; set; }

        public string DeductibleType { get; set; }
        public int? ODLimit { get; set; }
        public int? TPLLimit { get; set; }
        public decimal? PolicyPremium { get; set; }
        public decimal? AnnualPremiumBFD { get; set; }
    }
}
