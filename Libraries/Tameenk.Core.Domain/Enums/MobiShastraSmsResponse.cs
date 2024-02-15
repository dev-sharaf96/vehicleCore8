
using Newtonsoft.Json;

namespace Tameenk.Core.Domain.Enums
{
    public class MobiShastraSmsResponse
    {
        [JsonProperty("msg_id")]
        public string MessageId { get; set; }

        [JsonProperty("number")]
        public string Number { get; set; }

        [JsonProperty("str_response")]
        public string ResponseString { get; set; }
    }
}
