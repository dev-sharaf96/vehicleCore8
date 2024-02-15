using Newtonsoft.Json;

namespace Tameenk.Integration.Dto.Quotation
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