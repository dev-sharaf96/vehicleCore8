using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Integration.Dto.Quotation
{
    [JsonObject("product")]
    public class ProductModel
    {

        public ProductModel()
        {
            PriceDetails = new HashSet<PriceDetailModel>();
            Product_Benefits = new HashSet<ProductBenefitModel>();
        }

        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("externalProductId")]
        public string ExternalProductId { get; set; }

        [JsonProperty("quotaionNo")]
        public string QuotaionNo { get; set; }

        [JsonProperty("quotationDate")]
        public DateTime? QuotationDate { get; set; }

        [JsonProperty("quotationExpiryDate")]
        public DateTime? QuotationExpiryDate { get; set; }

        [JsonProperty("providerId")]
        public int? ProviderId { get; set; }

        [JsonProperty("productNameAr")]
        public string ProductNameAr { get; set; }

        [JsonProperty("productNameEn")]
        public string ProductNameEn { get; set; }

        [JsonProperty("productDescAr")]
        public string ProductDescAr { get; set; }

        [JsonProperty("productDescEn")]
        public string ProductDescEn { get; set; }

        [JsonProperty("productPrice")]
        public decimal ProductPrice { get; set; }

        [JsonProperty("deductableValue")]
        public int? DeductableValue { get; set; }

        [JsonProperty("vehicleLimitValue")]
        public int? VehicleLimitValue { get; set; }

        [JsonProperty("quotationResponseId")]
        public int? QuotationResponseId { get; set; }

        [JsonProperty("productImage")]
        public string ProductImage { get; set; }

        [JsonProperty("insuranceTypeCode")]
        public int InsuranceTypeCode { get; set; }



        [JsonProperty("priceDetails")]
        public ICollection<PriceDetailModel> PriceDetails { get; set; }

        [JsonProperty("productBenefits")]
        public ICollection<ProductBenefitModel> Product_Benefits { get; set; }

        [JsonProperty("isPromoted")]
        public bool IsPromoted { get; set; }

        [JsonProperty("shadowAmount")]
        public decimal? ShadowAmount { get; set; } = 0;

        [JsonProperty("insuranceRate")]
        public decimal? InsurancePercentage { get; set; }

        [JsonProperty("termsFilePathEn")]
        public string TermsFilePathEn { get; set; }
        [JsonProperty("termsFilePathAr")]
        public string TermsFilePathAr { get; set; }
        [JsonIgnore]
        public string DeductibleType { get; set; }
        [JsonIgnore]
        public int? ODLimit { get; set; }
        [JsonIgnore]
        public int? TPLLimit { get; set; }
        [JsonIgnore]
        public decimal? PolicyPremium { get; set; }
        [JsonIgnore]
        public decimal? AnnualPremiumBFD { get; set; }
        public bool ShowTabby { get; set; }
        public bool IsRenewal { get; set; }
    }
}
