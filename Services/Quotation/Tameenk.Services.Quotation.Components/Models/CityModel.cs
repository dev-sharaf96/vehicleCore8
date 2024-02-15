using Newtonsoft.Json;

namespace Tameenk.Services.Quotation.Components
{
    /// <summary>
    /// Represent City
    /// </summary>
    [JsonObject("city")]
    public class CityModel
    {
        /// <summary>
        /// English Description
        /// </summary>
        [JsonProperty("englishDescription")]
        public string EnglishDescription { get; set; }

        /// <summary>
        /// Arabic Description
        /// </summary>
        [JsonProperty("arabicDescription")]
        public string ArabicDescription { get; set; }
    }
}