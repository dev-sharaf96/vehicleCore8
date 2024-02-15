
using Newtonsoft.Json;

namespace Tameenk.Services.Implementation.Wathq
{
   public class BusinessType
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
