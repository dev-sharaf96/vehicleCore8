using Newtonsoft.Json;

namespace Tameenk.Services.Quotation.Components
{
    [JsonObject("errorModel")]
    public class ErrorModel
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("field")]
        public string Field { get; set; }
    }
}