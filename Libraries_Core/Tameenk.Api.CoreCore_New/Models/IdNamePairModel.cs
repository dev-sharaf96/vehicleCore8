//using Newtonsoft.Json;

using Newtonsoft.Json;

namespace Tameenk.Api.Core.Models
{
    /// <summary>
    /// Id Name Pair Model
    /// </summary>
    [JsonObject("idNamePair")]
    public class IdNamePairModel
    {
        /// <summary>
        /// Id
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        [JsonProperty("name")]
        public string? Name { get; set; }
    }
}