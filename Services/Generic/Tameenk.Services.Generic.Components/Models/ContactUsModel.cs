using Newtonsoft.Json;
using Tameenk.Common.Utilities; 

namespace Tameenk.Services.Generic.Components.Models
{
    public class ContactUsModel  
    {
        [JsonProperty("nin")]
        public string Nin { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("language")]
        public string Language { get; set; } = "ar";

        [JsonProperty("channel")]
        public Channel Channel { get; set; } = Channel.Portal;
    }
}
