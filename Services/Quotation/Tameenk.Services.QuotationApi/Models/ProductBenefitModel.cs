using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.QuotationApi.Models
{
    [JsonObject("productBenefit")]
    public class ProductBenefitModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("productId")]
        public Guid? ProductId { get; set; }

        [JsonProperty("benefitId")]
        public short? BenefitId { get; set; }

        [JsonProperty("isSelected")]
        public bool? IsSelected { get; set; }

        [JsonProperty("benefitPrice")]
        public decimal? BenefitPrice { get; set; }

        [JsonProperty("benefitExternalId")]
        public string BenefitExternalId { get; set; }

        [JsonProperty("isReadOnly")]
        public bool IsReadOnly { get; set; }

        [JsonProperty("benefit")]
        public BenefitModel Benefit { get; set; }

        
    }
}