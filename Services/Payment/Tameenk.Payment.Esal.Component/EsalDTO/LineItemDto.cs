using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Payment.Esal.Component
{
    public class LineItemDto
    {
        [JsonProperty("serialNumber")]
        public long SerialNumber { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("descriptionEnglish")]
        public string DescriptionEnglish { get; set; }
        [JsonProperty("descriptionArabic")]
        public string DescriptionArabic { get; set; }
        [JsonProperty("uomEnglish")]
        public string UomEnglish { get; set; }
        [JsonProperty("uomArabic")]
        public string UomArabic { get; set; }
        [JsonProperty("price")]
        public decimal Price { get; set; }
        [JsonProperty("quantity")]
        public int Quantity { get; set; }
        [JsonProperty("amount")]
        public decimal Amount { get; set; }
        [JsonProperty("discountPercent")]
        public decimal DiscountPercent { get; set; }
        [JsonProperty("discountAmount")]
        public decimal DiscountAmount { get; set; }
        [JsonProperty("amountAfterDiscount")]
        public decimal AmountAfterDiscount { get; set; }
        [JsonProperty("vatPercent")]
        public decimal VatPercent { get; set; }
        [JsonProperty("totalVat")]
        public decimal TotalVat { get; set; }
        [JsonProperty("priceAfterVat")]
        public decimal PriceAfterVat { get; set; }
    }
}
