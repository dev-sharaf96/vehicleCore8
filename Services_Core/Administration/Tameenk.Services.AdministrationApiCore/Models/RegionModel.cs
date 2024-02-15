using Newtonsoft.Json;

namespace Tameenk.Services.AdministrationApi.Models
{
    [JsonObject("region")]
    public class RegionModel
    {
        /// <summary>
        /// City code.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}