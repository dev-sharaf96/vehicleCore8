using Newtonsoft.Json;

namespace Tameenk.Services.QuotationApi.Models
{
    [JsonObject("benefit")]
    public class BenefitModel
    {
        [JsonProperty("code")]
        public short Code { get; set; }

        [JsonProperty("englishDescription")]
        public string EnglishDescription { get; set; }

        [JsonProperty("arabicDescription")]
        public string ArabicDescription { get; set; }
    }
}