using Newtonsoft.Json;

namespace Tameenk.Core.Domain.Dtos
{
    public class AddItemToCartResponse
    {
        [JsonProperty("qtRqstExtrnlId")]
        public string QtRqstExtrnlId { get; set; }

        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }
    }
}
