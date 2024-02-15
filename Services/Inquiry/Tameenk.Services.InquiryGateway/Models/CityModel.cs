using Newtonsoft.Json;

namespace Tameenk.Services.InquiryGateway.Models
{
    [JsonObject("city")]
    public class CityModel
    {
        /// <summary>
        /// City code.
        /// </summary>
        [JsonProperty("code")]
        public long Code { get; set; }
        /// <summary>
        /// English Description.
        /// </summary>
        [JsonProperty("englishDescription")]
        public string EnglishDescription { get; set; }
        /// <summary>
        /// Arabic Description.
        /// </summary>
        [JsonProperty("arabicDescription")]
        public string ArabicDescription { get; set; }
    }
}