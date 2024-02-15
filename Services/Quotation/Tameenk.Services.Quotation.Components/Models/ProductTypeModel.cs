using Newtonsoft.Json;

namespace Tameenk.Services.Quotation.Components
{
    [JsonObject("productType")]
    public class ProductTypeModel
    {
        [JsonProperty("code")]
        public short Code { get; set; }

        [JsonProperty("englishDescription")]
        public string EnglishDescription { get; set; }

        [JsonProperty("arabicDescription")]
        public string ArabicDescription { get; set; }
    }
}