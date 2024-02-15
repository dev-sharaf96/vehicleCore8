using Newtonsoft.Json;

namespace Tameenk.Core.Domain.Dtos
{
    public class InquiryLookup
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("nameEn", NullValueHandling = NullValueHandling.Ignore)]
        public string NameEn { get; set; }
    }
}
