using Newtonsoft.Json;

namespace Tameenk.Services.InquiryGateway.Models
{
    [JsonObject("country")]
    public class CountryModel
    {
        [JsonProperty("code")]
        public short Code { get; set; }

        [JsonProperty("englishDescription")]

        public string EnglishDescription { get; set; }


        [JsonProperty("arabicDescription")]
        public string ArabicDescription { get; set; }
    }
}