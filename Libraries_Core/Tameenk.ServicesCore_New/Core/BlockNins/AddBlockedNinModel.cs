//using Newtonsoft.Json;

using Newtonsoft.Json;

namespace Tameenk.Services.Core.BlockNins
{
    public class AddBlockedNinModel
    {
        [JsonProperty("nationalId")]
        public string? NationalId { get; set; }
        [JsonProperty("lang")]
        public string? Lang { get; set; }
        [JsonProperty("blockReason")]
        public string? BlockReason { get; set; }
    }
}
