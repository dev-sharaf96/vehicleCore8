using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Services.Policy.Components
{
    public class OrderItemInfo
    {
        public int Id { get; set; }
        public string CheckoutDetailReferenceId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }

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
        public bool IsPromoted { get; set; }
        public string DeductibleType { get; set; }
        public int? ODLimit { get; set; }
        public int? TPLLimit { get; set; }
        public decimal? PolicyPremium { get; set; }
        public decimal? AnnualPremiumBFD { get; set; }
    }
}
