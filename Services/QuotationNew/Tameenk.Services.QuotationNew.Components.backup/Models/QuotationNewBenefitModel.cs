using Newtonsoft.Json;

namespace Tameenk.Services.QuotationNew.Components
{
    [JsonObject("benefit")]
    public class QuotationNewBenefitModel
    {
        [JsonProperty("code")]
        public short Code { get; set; }

        [JsonProperty("englishDescription")]
        public string EnglishDescription { get; set; }

        [JsonProperty("arabicDescription")]
        public string ArabicDescription { get; set; }
    }
}