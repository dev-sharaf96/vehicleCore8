using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tameenk.Services.QuotationNew.Components
{
    [JsonObject("priceDetail")]
    public class QuotationNewPriceDetailModel
    {
        [JsonProperty("detailId")]
        public Guid DetailId { get; set; }

        [JsonProperty("productID")]
        public Guid ProductID { get; set; }

        [JsonProperty("priceTypeCode")]
        public byte PriceTypeCode { get; set; }

        [JsonProperty("priceValue")]
        public decimal PriceValue { get; set; }

        [JsonProperty("percentageValue")]
        public decimal? PercentageValue { get; set; }

        [JsonProperty("priceType")]
        public QuotationNewPriceTypeModel PriceType { get; set; }
    }
}